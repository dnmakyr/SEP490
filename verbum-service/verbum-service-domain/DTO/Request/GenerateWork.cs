namespace verbum_service_domain.DTO.Request
{
    public class GenerateWork
    {
        public Guid OrderId { get; set; }
        public string OrderName { get; set; }
        public DateTime? DueDate { get; set; }
        public bool HasTranslateService { get; set; }
        public bool HasEditService { get; set; }
        public bool HasEvaluateService { get; set; }
    }
}
