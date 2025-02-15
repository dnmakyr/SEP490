namespace verbum_service_domain.DTO.Request
{
    public class UpdateIssueRequest
    {
        public Guid IssueId { get; set; }
        public string IssueName { get; set; }
        public string IssueDescription { get; set; }
        public Guid? AssigneeId { get; set; }
        //list attachment
        public List<UpdateIssueAttachmentFile>? IssueAttachments { get; set; } = new List<UpdateIssueAttachmentFile>();
    }
}
