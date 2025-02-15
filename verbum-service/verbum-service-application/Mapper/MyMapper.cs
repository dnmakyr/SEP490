using AutoMapper;
using verbum_service_domain.Common;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.DTO.Response;
using verbum_service_domain.Models;

namespace verbum_service_application.Mapper
{
    public class MyMapper : Profile
    {
        public MyMapper()
        {
            CreateMap<User, UserInfo>().ReverseMap();
            CreateMap<UserSignUp, User>().ReverseMap();
            CreateMap<Category, CategoryInfoResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CategoryName));
            CreateMap<Category, CategoryInfo>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CategoryName))
                .ReverseMap();
            CreateMap<Category, CategoryUpdate>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CategoryName))
                .ReverseMap();
            CreateMap<Issue, CreateIssueRequest>().ReverseMap();
            CreateMap<Issue, UpdateIssueRequest>().ReverseMap();
            CreateMap<Issue, ReopenIssueRequest>().ReverseMap();
            CreateMap<Issue, IssueResponse>()
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.Name))
                .ForMember(dest => dest.AssigneeName, opt => opt.MapFrom(src => src.Assignee.Name))
                .ForMember(dest => dest.IssueAttachments, opt => opt.MapFrom(src => src.IssueAttachments.Where(a => !a.IsDeleted)))
                .ForMember(dest => dest.DocumentUrl, opt => opt.MapFrom(src => src.Job.DocumentUrl))
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Job.Work.OrderId))
                .ForMember(dest => dest.OrderName, opt => opt.MapFrom(src => src.Job.Work.Order.OrderName))
                .ReverseMap();
            CreateMap<IssueAttachment, UploadIssueAttachmentFiles>().ReverseMap();
            CreateMap<IssueAttachment, UpdateIssueAttachmentFile>().ReverseMap();
            CreateMap<Language, LanguageResponse>().ReverseMap();
            CreateMap<UpdateLanguageSupportRequest, Language>()
                .ForMember(dest => dest.LanguageId, opt => opt.MapFrom(src => src.LanguageId.ToUpper())).ReverseMap();
            CreateMap<Order, OrderCreate>().ReverseMap();
            CreateMap<Order, OrderResponse>().ReverseMap();
            CreateMap<Order, OrderDetailsResponse>()
                .ForMember(dest => dest.TargetLanguageId, opt => opt.MapFrom(src => src.TargetLanguages.Select(t => t.LanguageId).ToList()))
                .ForMember(dest => dest.ReferenceFileUrls, opt => opt.MapFrom(src => src.OrderReferences.Where(t => t.Tag == OrderFileTag.REFERENCES.ToString() && t.IsDeleted == false).Select(t => t.ReferenceFileUrl).ToList()))
                .ForMember(dest => dest.TranslationFileUrls, opt => opt.MapFrom(src => src.OrderReferences.Where(t => t.Tag == OrderFileTag.TRANSLATION.ToString() && t.IsDeleted == false).Select(t => t.ReferenceFileUrl).ToList()))
                .ForMember(dest => dest.JobDeliverables, opt => opt.MapFrom(src => src.Works.SelectMany(work => work.Jobs.Where(j => j.DeliverableUrl != null).Select(job => new JobDeliverableResponse
                {
                    DeliverableFileUrl = job.DeliverableUrl,
                    ServiceOrder = work.ServiceCodeNavigation.ServiceOrder,
                }))))
                .ForMember(dest => dest.DeleteddFileUrls, opt => opt.MapFrom(src => src.OrderReferences.Where(t => t.IsDeleted == true).Select(t => t.ReferenceFileUrl).ToList()))
                .ForMember(dest => dest.CompletedDate, opt => opt.MapFrom(src => src.Receipts.Where(t => t.DepositeOrPayment == false).Select(t => t.PayDate).FirstOrDefault()))
                .ForMember(dest => dest.DiscountName, opt => opt.MapFrom(src => src.Discount.DiscountName))
                .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.Discount.DiscountPercent));
            CreateMap<OrderReference, UploadOrderFileRequest>().ReverseMap();
            CreateMap<Work, WorkResponse>()
                .ForMember(dest => dest.SourceLanguageId, opt => opt.MapFrom(src => src.Order.SourceLanguageId))
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.Order.OrderStatus))
                .ForMember(dest => dest.TargetLanguageId, opt => opt.MapFrom(src => src.Order.TargetLanguages.Select(t => t.LanguageId).ToList()))
                .ForMember(dest => dest.ReferenceFileUrls, opt => opt.MapFrom(src => src.Order.OrderReferences.Where(t => t.Tag == OrderFileTag.REFERENCES.ToString()).Select(t => t.ReferenceFileUrl).ToList()))
                .ForMember(dest => dest.TranslationFileUrls, opt => opt.MapFrom(src => src.Order.OrderReferences.Where(t => t.Tag == OrderFileTag.TRANSLATION.ToString()).Select(t => t.ReferenceFileUrl).ToList()))
                .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.Jobs.All(job => job.Status.Equals(JobStatus.APPROVED.ToString()))))
                .ForMember(dest => dest.TranslateService, opt => opt.MapFrom(src => src.Order.HasTranslateService))
                .ForMember(dest => dest.EditService, opt => opt.MapFrom(src => src.Order.HasEditService))
                .ForMember(dest => dest.EvaluateService, opt => opt.MapFrom(src => src.Order.HasEvaluateService));
            CreateMap<Work, WorkCreate>().ReverseMap();
            CreateMap<Work, WorkUpdate>().ReverseMap();
            CreateMap<Discount, DiscountDTO>().ReverseMap();
            CreateMap<Discount, DiscountResponse>().ReverseMap();
            CreateMap<Rating, RatingResponse>().ReverseMap();
            CreateMap<Rating, RatingCreate>().ReverseMap();
            CreateMap<Rating, RatingUpdate>().ReverseMap();
            CreateMap<Job, JobInfoResponse>()
                .ForMember(dest => dest.WorkDueDate, opt => opt.MapFrom(src => src.Work.DueDate))
                .ForMember(dest => dest.AssigneeNames, opt => opt.MapFrom(src => src.Assignees.ToList()))
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Work.OrderId))
                .ForMember(dest => dest.ReferenceUrls, opt => opt.MapFrom(src => src.Work.Order.OrderReferences.Where(t => t.Tag == OrderFileTag.REFERENCES.ToString()).Select(x => x.ReferenceFileUrl).ToList()))
                .ForMember(dest => dest.ServiceOrder, opt => opt.MapFrom(src => src.Work.ServiceCodeNavigation.ServiceOrder))
                .ReverseMap();
            CreateMap<Job, JobListResponse>()
                .ForMember(dest => dest.AssigneeNames, opt => opt.MapFrom(src => src.Assignees.ToList()))
                .ReverseMap();
            CreateMap<Job, UpdateJobRequest>()
                .ForMember(dest => dest.AssigneesId, opt => opt.MapFrom(src => src.Assignees.Select(x => x.Id).ToList()))
                .ReverseMap();
            CreateMap<Receipt, ReceiptInfoResponse>()
                .ForMember(dest => dest.OrderName, opt => opt.MapFrom(src => src.Order.OrderName))
                .ReverseMap();
            CreateMap<Receipt, CreateReceipRequest>().ReverseMap();
        }
    }
}
