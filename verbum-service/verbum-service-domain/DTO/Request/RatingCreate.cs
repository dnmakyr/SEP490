namespace verbum_service_domain.DTO.Request
{
    public class RatingCreate
    {
        public Guid OrderId { get; set; }
        public decimal? InTime { get; set; }
        public decimal? Expectation { get; set; }
        public decimal? IssueResolved { get; set; }
        public string? MoreThought { get; set; }
    }
}
