using System.ComponentModel.DataAnnotations;

namespace verbum_service_domain.DTO.Response
{
    public class WorkResponse
    {
        [Key]
        public Guid WorkId { get; set; }
        public string WorkName { get; set; }
        public DateTime? DueDate { get; set; }
        public string SourceLanguageId { get; set; }
        public List<string> TargetLanguageId { get; set; }
        public List<string> TranslationFileUrls { get; set; }
        public List<string> ReferenceFileUrls { get; set; }
        public string OrderStatus { get; set; }
        public string? ServiceCode { get; set; }
        public bool IsCompleted { get; set; }
        public bool TranslateService {  get; set; }
        public bool EditService { get; set; }
        public bool EvaluateService { get; set; }
    }
}
