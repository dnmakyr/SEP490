namespace verbum_service_domain.DTO.Request
{
    public class OrderCreate
    {
        public string SourceLanguageId { get; set; }
        public List<string> TargetLanguageIdList { get; set; }
        public bool HasTranslateService { get; set; }
        public bool HasEditService { get; set; }
        public bool HasEvaluateService { get; set; }
        public string? OrderNote { get; set; }
        public List<string> TranslationFileURL { get; set; } //url input and upload files urls
        public List<string>? ReferenceFileURLs { get; set; } //url input and upload files urls
        public DateTime? DueDate { get; set; }
        public Guid? DiscountId { get; set; }
    }
}
