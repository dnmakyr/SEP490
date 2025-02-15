namespace verbum_service_domain.DTO.Request
{
    public class RatingUpdate
    {
        public Guid RatingId { get; set; }
        public decimal? InTime { get; set; }
        public decimal? Expectation { get; set; }
        public decimal? IssueResolved { get; set; }
        public string? MoreThought { get; set; }
    }
}
