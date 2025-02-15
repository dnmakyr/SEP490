namespace verbum_service_domain.DTO.Request
{
    public class UploadOrderFileRequest
    {
        public Guid OrderId { get; set; }

        public string ReferenceFileUrl { get; set; } = null!;

        public string Tag { get; set; } //TRANSLATION, REFERENCES, DELIVERABLES
    }
}
