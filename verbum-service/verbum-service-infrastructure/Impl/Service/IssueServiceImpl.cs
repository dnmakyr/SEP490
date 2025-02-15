using AutoMapper;
using Lombok.NET;
using MailKit;
using MailKit.Search;
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

namespace verbum_service_infrastructure.Impl.Service
{
    [RequiredArgsConstructor]
    public partial class IssueServiceImpl : IssueService
    {
        private readonly IMapper mapper;
        private readonly verbumContext context;
        private readonly CurrentUser currentUser;
        private readonly verbum_service_application.Service.MailService mailService;

        public async Task AcceptIssueSolution(Guid issueId)
        {
            using (IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Issue issue = await context.Issues.Include(x => x.Assignee).FirstOrDefaultAsync(x => x.IssueId == issueId);
                    Guid orderId = await context.Issues.Where(x => x.IssueId == issueId)
                        .Include(x => x.Job)
                        .ThenInclude(x => x.Work)
                        .ThenInclude(x => x.Order).Select(x => x.Job.Work.Order.OrderId).FirstOrDefaultAsync();

                    string solutionUrl = await context.IssueAttachments.Where(x => x.IssueId == issueId && x.Tag == IssueFileTag.SOLUTION.ToString()).Select(x => x.AttachmentUrl).FirstOrDefaultAsync();

                    if (ObjectUtils.IsEmpty(solutionUrl)) throw new BusinessException(AlertMessage.Alert(ValidationAlertCode.NOT_FOUND, "solution for this issue"));

                    await context.Jobs.Where(x => x.Id.Equals(issue.JobId)).ExecuteUpdateAsync(x => x.SetProperty(u => u.DeliverableUrl, solutionUrl));
                    await context.Issues.Where(x => x.IssueId.Equals(issueId)).ExecuteUpdateAsync(x => x.SetProperty(u => u.Status, IssueStatusEnum.RESOLVED.ToString()));
                    await context.Issues.Where(x => x.IssueId.Equals(issueId)).ExecuteUpdateAsync(x => x.SetProperty(u => u.RejectResponse, (string?)null));

                    var jobs = await context.Jobs
                        .Include(x => x.Work)
                        .Include(x => x.Issue)
                        .Where(x => x.Work.OrderId == orderId)
                        .ToListAsync();

                    bool allCompleted = jobs.All(job =>
                    job.Status == JobStatus.APPROVED.ToString() &&
                    (job.Issue == null || job.Issue.Status == IssueStatusEnum.RESOLVED.ToString() || job.Issue.Status == IssueStatusEnum.CANCEL.ToString()));

                    //send mail
                    string mailBody = await MailUtils.BuildVerificationEmail(SystemConfig.FE_DOMAIN + "/issues?issueId=" + issueId, "accept_issue_solution");
                    _ = Task.Run(() => mailService.SendEmailAsync(issue.Assignee.Email, string.Format(MailConstant.ACCEPT_ISSUE_HEADER, issue.IssueName), mailBody));

                    if (allCompleted)
                    {
                        int orderRecords = await context.Orders
                            .Where(o => o.OrderId == orderId)
                            .ExecuteUpdateAsync(x => x.SetProperty(u => u.OrderStatus, OrderStatus.COMPLETED.ToString()));

                        if (orderRecords < 1) throw new BusinessException(ValidationAlertCode.UPDATE_RECORD_FAIL);

                        verbum_service_domain.Models.Order order = await context.Orders.Include(x => x.Client).FirstAsync(x => x.OrderId == orderId);
                        string body = await MailUtils.BuildVerificationEmail(SystemConfig.FE_DOMAIN + "/orders/details/" + order.OrderId, "complete_order_mail");
                        _ = Task.Run(() => mailService.SendEmailAsync(order.Client.Email, string.Format(MailConstant.COMPLETE_ORDER_HEADER, order.OrderName), mailBody));
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task AddIssue(CreateIssueRequest request)
        {
            Guid jobId = await context.Jobs.Where(x => x.DeliverableUrl.Equals(request.DeliverableUrl)).Select(x => x.Id).FirstOrDefaultAsync();

            Issue issue = mapper.Map<Issue>(request);
            issue.IssueId = Guid.NewGuid();
            issue.CreatedAt = DateTime.Now;
            issue.UpdatedAt = DateTime.Now;
            issue.Status = IssueStatusEnum.OPEN.ToString();
            issue.ClientId = currentUser.Id;
            issue.JobId = jobId;
            issue.SrcDocumentUrl = request.DeliverableUrl;
            context.Issues.Add(issue);

            if (await context.SaveChangesAsync() < 1) throw new BusinessException(ValidationAlertCode.UPDATE_RECORD_FAIL);

            int orderRecords = await context.Orders
                            .Where(o => o.OrderId == request.OrderId)
                            .ExecuteUpdateAsync(x => x.SetProperty(u => u.OrderStatus, OrderStatus.IN_PROGRESS.ToString()));
        }

        public async Task DeleteIssueAttachmentFile(Guid issueId, string attachmentUrl)
        {
            int records = await context.IssueAttachments
                .Where(x => x.IssueId == issueId && x.AttachmentUrl == attachmentUrl)
                .ExecuteUpdateAsync(x => x.SetProperty(u => u.IsDeleted, true));
            if (records < 1) throw new BusinessException(ValidationAlertCode.UPDATE_RECORD_FAIL);
        }

        public async Task<List<UploadIssueAttachmentFiles>> GetAllIssueAttachments()
        {
            return mapper.Map<List<UploadIssueAttachmentFiles>>(await context.IssueAttachments.Where(x => !x.IsDeleted).ToListAsync());
        }

        public async Task UpdateIssue(UpdateIssueRequest request)
        {
            Issue? updateIssue = await context.Issues.Include(x => x.IssueAttachments).FirstOrDefaultAsync(x => x.IssueId == request.IssueId);
            updateIssue.UpdatedAt = DateTime.Now;
            updateIssue.IssueAttachments = mapper.Map<List<IssueAttachment>>(request.IssueAttachments);
            updateIssue.IssueName = request.IssueName;
            updateIssue.IssueDescription = request.IssueDescription;
            updateIssue.AssigneeId = request.AssigneeId;
            context.Issues.Update(updateIssue);
            int records = await context.SaveChangesAsync();
            if (records < 1) throw new BusinessException(ValidationAlertCode.UPDATE_RECORD_FAIL);
        }

        public async Task ReopenIssue(ReopenIssueRequest request)
        {
            Issue? updateIssue = await context.Issues.Include(x => x.IssueAttachments).FirstOrDefaultAsync(x => x.IssueId == request.IssueId);
            if (updateIssue.Status != IssueStatusEnum.CANCEL.ToString() && updateIssue.Status != IssueStatusEnum.RESOLVED.ToString()) throw new BusinessException(AlertMessage.Alert(ValidationAlertCode.INVALID, "issue status"));
            if (await context.Issues.AnyAsync(x => x.IssueName.Equals(request.IssueName) && !x.IssueId.Equals(request.IssueId))) throw new BusinessException(AlertMessage.Alert(ValidationAlertCode.DUPLICATE, "issue name"));

            updateIssue.UpdatedAt = DateTime.Now;
            updateIssue.Status = IssueStatusEnum.OPEN.ToString();
            updateIssue.IssueAttachments = mapper.Map<List<IssueAttachment>>(request.IssueAttachments);
            updateIssue.CancelResponse = null;
            context.Issues.Update(updateIssue);

            Guid orderId = await context.Issues.Where(x => x.IssueId == request.IssueId)
                        .Include(x => x.Job)
                        .ThenInclude(x => x.Work)
                        .ThenInclude(x => x.Order).Select(x => x.Job.Work.Order.OrderId).FirstOrDefaultAsync();

            int records = await context.SaveChangesAsync();
            if (records < 1) throw new BusinessException(ValidationAlertCode.UPDATE_RECORD_FAIL);

            await context.Orders
                            .Where(o => o.OrderId == orderId)
                            .ExecuteUpdateAsync(x => x.SetProperty(u => u.OrderStatus, OrderStatus.IN_PROGRESS.ToString()));
        }

        public async Task UpdateIssueCancelResponse(ResponseRequest request)
        {
            if (await context.Issues.Where(x => x.IssueId == request.Id)
                .ExecuteUpdateAsync(o => o.SetProperty(x => x.CancelResponse, request.ResponseContent)) < 1) throw new BusinessException(ValidationAlertCode.UPDATE_RECORD_FAIL);
            Issue issue = await context.Issues.Include(x => x.Assignee).FirstAsync(x => x.IssueId == request.Id);

            Guid orderId = await context.Issues.Where(x => x.IssueId == request.Id)
                .Include(x => x.Job)
                .ThenInclude(x => x.Work)
                .ThenInclude(x => x.Order).Select(x => x.Job.Work.Order.OrderId).FirstOrDefaultAsync();

            var jobs = await context.Jobs
                        .Include(x => x.Work)
                        .Include(x => x.Issue)
                        .Where(x => x.Work.OrderId == orderId)
                        .ToListAsync();

            bool allCompleted = jobs.All(job =>
                    job.Status == JobStatus.APPROVED.ToString() &&
                    (job.Issue == null || job.Issue.Status == IssueStatusEnum.RESOLVED.ToString() || job.Issue.Status == IssueStatusEnum.CANCEL.ToString()));

            if (allCompleted)
            {
                int orderRecords = await context.Orders
                    .Where(o => o.OrderId == orderId)
                    .ExecuteUpdateAsync(x => x.SetProperty(u => u.OrderStatus, OrderStatus.COMPLETED.ToString()));

                if (orderRecords < 1) throw new BusinessException(ValidationAlertCode.UPDATE_RECORD_FAIL);

                verbum_service_domain.Models.Order order = await context.Orders.Include(x => x.Client).FirstAsync(x => x.OrderId == orderId);
                string orderbody = await MailUtils.BuildVerificationEmail(SystemConfig.FE_DOMAIN + "/orders/details/" + order.OrderId, "complete_order_mail");
                _ = Task.Run(() => mailService.SendEmailAsync(order.Client.Email, string.Format(MailConstant.COMPLETE_ORDER_HEADER, order.OrderName), orderbody));
            }


            //send mail cancel issue
            string body = await MailUtils.BuildVerificationEmail(SystemConfig.FE_DOMAIN + "/issues?issueId=" + request.Id, "cancel_issue_mail", request.ResponseContent);
            _ = Task.Run(() => mailService.SendEmailAsync(issue.Assignee.Email, string.Format(MailConstant.CANCEL_ISSUE_HEADER, issue.IssueName), body));
        }

        public async Task UpdateIssueRejectResponse(ResponseRequest request)
        {
            if (await context.Issues.Where(x => x.IssueId == request.Id)
                .ExecuteUpdateAsync(o => o.SetProperty(x => x.RejectResponse, request.ResponseContent)) < 1) throw new BusinessException(ValidationAlertCode.UPDATE_RECORD_FAIL);
            Issue issue = await context.Issues.Include(x => x.Assignee).FirstAsync(x => x.IssueId == request.Id);
            string mailBody = await MailUtils.BuildVerificationEmail(SystemConfig.FE_DOMAIN + "/issues?issueId=" + request.Id, "reject_issue_mail", request.ResponseContent);
            _ = Task.Run(() => mailService.SendEmailAsync(issue.Assignee.Email, string.Format(MailConstant.REJECT_ISSUE_HEADER, issue.IssueName), mailBody));
        }

        public async Task UpdateIssueStatus(Guid issueId, string status)
        {
            if (!Enum.IsDefined(typeof(IssueStatusEnum), status)
                || IssueStatusEnum.OPEN.ToString().Equals(status)
                || (UserRole.CLIENT.Equals(currentUser.Role) && !IssueStatusEnum.CANCEL.ToString().Equals(status)))
            {
                throw new BusinessException(AlertMessage.Alert(ValidationAlertCode.INVALID, "issue status"));
            }
            if (await context.Issues.Where(x => x.IssueId.Equals(issueId)).ExecuteUpdateAsync(o => o.SetProperty(a => a.Status, status)) < 1)
            {
                throw new BusinessException(ValidationAlertCode.UPDATE_RECORD_FAIL);
            }
        }

        public async Task UploadIssueAttachment(List<UploadIssueAttachmentFiles> attachmentFiles)
        {
            using (IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.IssueAttachments.AddRange(mapper.Map<List<IssueAttachment>>(attachmentFiles));
                    int records = await context.SaveChangesAsync();
                    if (records != attachmentFiles.Count) throw new BusinessException(ValidationAlertCode.UPDATE_RECORD_FAIL);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task<List<IssueResponse>> ViewAllIssue()
        {
            List<Issue> issues = await context.Issues.Include(x => x.Assignee).Include(x => x.IssueAttachments).Include(x => x.Client).Include(x => x.Job).ThenInclude(x => x.Work).ThenInclude(x => x.Order).ToListAsync();
            switch (currentUser.Role)
            {
                case UserRole.CLIENT:
                    issues = issues
                        .Where(x => x.ClientId == currentUser.Id)
                        .ToList();
                    break;
                case UserRole.LINGUIST:
                    issues = issues
                        .Where(x => x.AssigneeId == currentUser.Id)
                        .ToList();
                    break;
                case UserRole.TRANSLATE_MANAGER:
                case UserRole.EDIT_MANAGER:
                case UserRole.EVALUATE_MANAGER:
                    break;
                default:
                    throw new BusinessException(AlertMessage.Alert(ValidationAlertCode.NOT_FOUND, "Role"));
            }
            return mapper.Map<List<IssueResponse>>(issues);
        }
    }
}
