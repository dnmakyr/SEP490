using verbum_service_domain.DTO.Request;
using verbum_service_domain.DTO.Response;

namespace verbum_service_application.Service
{
    public interface JobService
    {
        Task<List<JobListResponse>> GetAllJob();
        Task CreateJobs(CreateJobsRequest request);
        Task UpdateJob(UpdateJobRequest request);
        Task<JobInfoResponse> GetJobById (Guid jobId);
        Task ApproveJob(Guid jobId);
        Task RejectJob(ResponseRequest request);
    }
}
