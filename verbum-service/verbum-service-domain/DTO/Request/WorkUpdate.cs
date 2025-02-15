namespace verbum_service_domain.DTO.Request
{
    public class WorkUpdate
    {
        public Guid WorkId { get; set; }
        public string WorkName { get; set; }
        public List<int>? OldCategoryIds { get; set; }
        public List<string>? NewCategory { get; set; }
        public string? ServiceCode { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
