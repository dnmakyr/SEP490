using AutoMapper;
using Lombok.NET;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PayPal.Api;
using verbum_service_application.Service;
using verbum_service_domain.Common;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.DTO.Response;
using verbum_service_domain.Models;
using verbum_service_domain.Utils;
using verbum_service_infrastructure.DataContext;
using verbum_service_infrastructure.Impl.Validation;

namespace verbum_service_infrastructure.Impl.Service
{
    [RequiredArgsConstructor]
    public partial class JobServiceImpl : JobService
    {
        private readonly verbumContext context;
        private readonly IMapper mapper;
        private readonly UpdateJobValidation validation;
        private readonly verbum_service_application.Service.MailService mailService;
        private readonly CurrentUser currentUser;
        public async Task CreateJobs(CreateJobsRequest request)
        {
            using (IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    foreach (Guid workId in request.WorkIds)
                    {
                        Work work = await context.Works.Include(x => x.ServiceCodeNavigation).FirstOrDefaultAsync(w => w.WorkId == workId);
                        foreach (string docUrl in request.DocumentUrls)
                        {
                            foreach (string targetLangId in request.TargetLanguageIds)
                            {
                                Job job = new Job
                                {
                                    Id = Guid.NewGuid(),
                                    Name = targetLangId + "_" + work.ServiceCodeNavigation.ServiceName + "_VERBUM_" + docUrl,
                                    Status = JobStatus.NEW.ToString(),
                                    CreatedAt = DateTime.Now,
                                    UpdatedAt = DateTime.Now,
                                    WordCount = 0,
                                    WorkId = workId,
                                    DocumentUrl = docUrl,
                                    TargetLanguageId = targetLangId
                                };
                                context.Jobs.Add(job);
                            }
                        }
                    }
                    await context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task<List<JobListResponse>> GetAllJob()
        {
            List<Job> allJobs = await context.Jobs.Include(x => x.Assignees).Include(x => x.Work).ToListAsync();
            switch(currentUser.Role)
            {
                case UserRole.LINGUIST:
                    allJobs = allJobs.Where(x => x.Assignees.Select(x => x.Id).Contains(currentUser.Id)).ToList();
                    break;
                case UserRole.TRANSLATE_MANAGER:
                    allJobs = allJobs.Where(x => x.Work.ServiceCode == "TL").ToList();
                    break;
                case UserRole.EDIT_MANAGER:
                    allJobs = allJobs.Where(x => x.Work.ServiceCode == "ED").ToList();
                    break;
                case UserRole.EVALUATE_MANAGER:
                    allJobs = allJobs.Where(x => x.Work.ServiceCode == "EV").ToList();
                    break;
            }
            return mapper.Map<List<JobListResponse>>(allJobs);
        }

        public async Task<JobInfoResponse> GetJobById(Guid jobId)
        {
            JobInfoResponse job = mapper.Map<JobInfoResponse>(await context.Jobs.Include(x => x.Assignees).Include(x => x.Issue).Include(x => x.Work).ThenInclude(x => x.ServiceCodeNavigation).Include(x => x.Work).ThenInclude(x => x.Order).ThenInclude(x => x.OrderReferences).FirstOrDefaultAsync(x => x.Id.Equals(jobId)));
            Dictionary<string, string> urls = await context.Jobs
                .Include(x => x.Work).ThenInclude(x => x.ServiceCodeNavigation)
                .Where(x => x.DocumentUrl == job.DocumentUrl && x.Work.ServiceCodeNavigation.ServiceOrder < job.ServiceOrder && !string.IsNullOrEmpty(x.DeliverableUrl) && x.TargetLanguageId.Equals(job.TargetLanguageId))
                .OrderBy(x => x.Work.ServiceCodeNavigation.ServiceOrder)
                .Select(x => new { x.DeliverableUrl, x.Work.ServiceCodeNavigation.ServiceName }) 
                .ToDictionaryAsync(x => x.DeliverableUrl ?? "", x => x.ServiceName);
            job.PreviousJobDeliverables = urls;
            return job;
        }

        public async Task UpdateJob(UpdateJobRequest request)
        {
            List<string> errors = await validation.Validate(request);
            if (ObjectUtils.IsNotEmpty(errors))
            {
                throw new BusinessException(errors);
            }
            Job job = await context.Jobs.Include(x => x.Assignees).FirstOrDefaultAsync(x => x.Id.Equals(request.Id));
            job.Name = request.Name;
            job.Status = request.Status;
            job.DeliverableUrl = request.DeliverableUrl;
            job.DueDate = request.DueDate;
            List<User> newAssignees = request.AssigneesId.Select(userId => new User { Id = userId }).ToList();
            job.Assignees = await context.Users
                .Where(user => request.AssigneesId.Contains(user.Id))
                .ToListAsync();
            if (await context.SaveChangesAsync() < 1)
            {
                throw new BusinessException(ValidationAlertCode.UPDATE_RECORD_FAIL);
            }
        }

        public async Task ApproveJob(Guid jobId)
        {
            int jobRecords = await context.Jobs
                .Where(x => x.Id == jobId)
                .ExecuteUpdateAsync(x => x.SetProperty(u => u.Status, JobStatus.APPROVED.ToString()).SetProperty(x => x.RejectReason, (string?) null));

            if (jobRecords < 1) throw new BusinessException(ValidationAlertCode.UPDATE_RECORD_FAIL);

            Job job = await context.Jobs
                        .Include(x => x.Work)
                        .ThenInclude(x => x.Order).ThenInclude(x => x.Client).Include(x => x.Assignees).FirstOrDefaultAsync(x => x.Id == jobId);


            var jobs = await context.Jobs
                .Include(x => x.Work)
                .Include(x => x.Issue)
                .Where(x => x.Work.OrderId == job.Work.OrderId)
                .ToListAsync();

            //send email
            string body = await MailUtils.BuildVerificationEmail(SystemConfig.FE_DOMAIN + "/jobs/details/" + jobId, "accept_job_mail");
            List<string> toEmail = job.Assignees.Select(x => x.Email).ToList();
            _ = Task.Run(() => mailService.SendEmailAsync(toEmail, MailConstant.ACCEPT_JOB_HEADER, body));

            bool allCompleted = jobs.All(job =>
            job.Status == JobStatus.APPROVED.ToString() &&
            (job.Issue == null || job.Issue.Status == IssueStatusEnum.RESOLVED.ToString() || job.Issue.Status == IssueStatusEnum.CANCEL.ToString()));

            if (allCompleted)
            {
                int orderRecords = await context.Orders
                    .Where(o => o.OrderId == job.Work.OrderId)
                    .ExecuteUpdateAsync(x => x.SetProperty(u => u.OrderStatus, OrderStatus.COMPLETED.ToString()));

                if (orderRecords < 1) throw new BusinessException(ValidationAlertCode.UPDATE_RECORD_FAIL);
                verbum_service_domain.Models.Order order = job.Work.Order;
                string mailBody = await MailUtils.BuildVerificationEmail(SystemConfig.FE_DOMAIN + "/orders/details/" + order.OrderId, "complete_order_mail");
                _ = Task.Run(() => mailService.SendEmailAsync(order.Client.Email, string.Format(MailConstant.COMPLETE_ORDER_HEADER, order.OrderName), body));
            }
        }

        public async Task RejectJob(ResponseRequest request)
        {
            if(context.Jobs.Where(x => x.Id == request.Id).Select(x => x.Status).First().Equals(JobStatus.APPROVED))
            {
                throw new BusinessException(AlertMessage.Alert(ValidationAlertCode.CANNOT_UPDATE, "job status"));
            }
            int records = await context.Jobs
                .Where(x => x.Id == request.Id)
                .ExecuteUpdateAsync(x => x.SetProperty(u => u.Status, JobStatus.IN_PROGRESS.ToString()).SetProperty(x => x.RejectReason, request.ResponseContent));
            if (records < 1) throw new BusinessException(ValidationAlertCode.UPDATE_RECORD_FAIL);
            //send mail
            List<string> assigneeEmails = context.Jobs.Include(x => x.Assignees).Where(job => job.Id == request.Id).SelectMany(job => job.Assignees).Select(assignee => assignee.Email).ToList();
            string mailBody = await MailUtils.BuildVerificationEmail(SystemConfig.FE_DOMAIN + "/jobs/details/" + request.Id, "reject_job_mail", request.ResponseContent);
            _ = Task.Run(() => mailService.SendEmailAsync(assigneeEmails, MailConstant.REJECT_JOB_HEADER, mailBody));
        }
    }
}
