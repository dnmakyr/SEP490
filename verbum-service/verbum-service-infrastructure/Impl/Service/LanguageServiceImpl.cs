using AutoMapper;
using Lombok.NET;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verbum_service_application.Service;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.DTO.Response;
using verbum_service_domain.Models;
using verbum_service_infrastructure.DataContext;

namespace verbum_service_infrastructure.Impl.Service
{
    [RequiredArgsConstructor]
    public partial class LanguageServiceImpl : LanguageService
    {
        private readonly verbumContext context;
        private readonly IMapper mapper;
        public async Task<List<LanguageResponse>> GetAllSupportedLanguages()
        {
            return mapper.Map<List<LanguageResponse>>(await context.Languages.Where(x => x.Support).ToListAsync());
        }
        public async Task<List<LanguageResponse>> GetAllLanguages()
        {
            return mapper.Map<List<LanguageResponse>>(await context.Languages.ToListAsync());
        }

        public async Task UpdateSupportLanguages(List<UpdateLanguageSupportRequest> languages)
        {
            using (IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Dictionary<string, bool> mapRequest = languages.ToDictionary(l => l.LanguageId.ToUpper(), l => l.Support);
                    foreach (Language lang in await context.Languages
                        .Where(l => mapRequest.Keys.Contains(l.LanguageId)).ToListAsync())
                    {
                        if (mapRequest.TryGetValue(lang.LanguageId.ToUpper(), out bool supportValue))
                        {
                            lang.Support = supportValue;
                        }
                    }
                    // Batch update using ExecuteUpdate
                    if (await context.SaveChangesAsync() < languages.Count)
                    {
                        throw new BusinessException(ValidationAlertCode.UPDATE_RECORD_FAIL);
                    }
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
