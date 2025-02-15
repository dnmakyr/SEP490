using AutoMapper;
using Lombok.NET;
using Microsoft.EntityFrameworkCore;
using verbum_service_application.Service;
using verbum_service_domain.Common;
using verbum_service_domain.Models;
using verbum_service_infrastructure.DataContext;

namespace verbum_service_infrastructure.Impl.Service
{
    public class ReferenceServiceImpl:ReferenceService
    {
        private readonly verbumContext context;
        private readonly IMapper mapper;
        private readonly CurrentUser currentUser;

        public ReferenceServiceImpl(verbumContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public bool AreAllUrlsValid(List<string> urls)
        {
            if (urls == null || urls.Count == 0)
            {
                return false; // No URLs to check
            }

            return urls.All(url => !string.IsNullOrWhiteSpace(url));
        }

        public async Task AddRange(Guid orderId, List<string> fileURLs, string tag)
        {
            if (AreAllUrlsValid(fileURLs))
            {
                try
                {
                    var references = fileURLs.Select(fileURL => new OrderReference
                    {
                        OrderId = orderId,
                        ReferenceFileUrl = fileURL,
                        Tag = tag,
                        IsDeleted = false
                    }).ToList();
                    context.OrderReferences.AddRange(references);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task UpdateReference(Guid orderId, List<string> fileURLs)
        {
            await context.OrderReferences.Where(c => c.OrderId == orderId).ExecuteDeleteAsync();
            await AddRange(orderId, fileURLs, "");
        }
    }
}
