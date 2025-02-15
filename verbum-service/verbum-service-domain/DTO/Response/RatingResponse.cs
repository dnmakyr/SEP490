namespace verbum_service_domain.DTO.Response
{
    public class RatingResponse
    {
        public Guid RatingId { get; set; }
        public Guid? OrderId { get; set; }
        public decimal? InTime { get; set; }
        public decimal? Expectation { get; set; }
        public decimal? IssueResolved { get; set; }
        public string? MoreThought { get; set; }
    }
}
