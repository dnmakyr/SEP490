namespace verbum_service_domain.DTO.Request
{
    public class WorkCreate
    {
        public Guid OrderId { get; set; }
        public string WorkName { get; set; }
        public List<int>? OldCategoryIds { get; set; }
        public List<string>? NewCategory {  get; set; }
        public string? ServiceCode { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
