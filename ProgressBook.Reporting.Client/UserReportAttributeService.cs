namespace ProgressBook.Reporting.Client
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using ProgressBook.Reporting.Client.Data;
    using ProgressBook.Reporting.Client.Models;
    using ProgressBook.Shared.Utilities;
    using ProgressBook.Shared.Utilities.Threading.Tasks;

    public interface IUserReportAttributeService : IDisposable
    {
        Task<int> TrackReportExecution(Guid reportEntityId, Guid userId, Guid districtId);
        Task<UserReportStats> GetUserReportStats(Guid reportEntityId, Guid userId, Guid districtId);
        Task SetFavorite(Guid reportEntityId, Guid userId, Guid districtId);
        Task UnsetFavorite(Guid reportEntityId, Guid userId, Guid districtId);
        Task<Guid> GetReportEntityId(string reportName, Guid userId, Guid districtId);
    }

    public class UserReportAttributeService : Disposable, IUserReportAttributeService
    {
        private readonly IAdHocReportsContext _dbContext;

        public UserReportAttributeService(IAdHocReportsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> TrackReportExecution(Guid reportEntityId, Guid userId, Guid districtId)
        {
            if (!_dbContext.ReportEntityInfo.Any(x => x.ReportEntityId == reportEntityId))
            {
                return 0;
            }

            var entity = await GetUserReportAttributeAsync(reportEntityId, userId, districtId);
            if (entity == null)
            {
                entity = new UserReportAttribute
                         {
                             ReportEntityId = reportEntityId,
                             UserId = userId,
                             DistrictId = districtId
                         };
                _dbContext.UserReportAttributes.Add(entity);
            }

            entity.LastRunDate = DateTime.Now;
            entity.RunCount += 1;
            await _dbContext.SaveChangesAsync();
            return entity.RunCount;
        }

        public async Task<UserReportStats> GetUserReportStats(Guid reportEntityId, Guid userId, Guid districtId)
        {
            var entity = await GetUserReportAttributeAsync(reportEntityId, userId, districtId);

            return entity == null
                       ? null
                       : new UserReportStats
                         {
                             IsFavorite = entity.IsFavorite,
                             LastRunDate = entity.LastRunDate,
                             RunCount = entity.RunCount
                         };
        }

        public async Task SetFavorite(Guid reportEntityId, Guid userId, Guid districtId)
        {
            if (!_dbContext.ReportEntityInfo.Any(x => x.ReportEntityId == reportEntityId))
            {
                return;
            }

            var entity = await GetUserReportAttributeAsync(reportEntityId, userId, districtId);
            if (entity == null)
            {
                entity = new UserReportAttribute
                         {
                             ReportEntityId = reportEntityId,
                             UserId = userId,
                             DistrictId = districtId
                         };
                _dbContext.UserReportAttributes.Add(entity);
            }

            entity.IsFavorite = true;
            await _dbContext.SaveChangesAsync();
        }

        public async Task UnsetFavorite(Guid reportEntityId, Guid userId, Guid districtId)
        {
            var entity = await GetUserReportAttributeAsync(reportEntityId, userId, districtId);
            if (entity == null)
            {
                return;
            }

            entity.IsFavorite = false;
            await _dbContext.SaveChangesAsync();
        }

        public Task<Guid> GetReportEntityId(string reportName, Guid userId, Guid districtId)
        {
            return _dbContext.ReportEntityInfo
                             .Where(x => x.Path == reportName)
                             .Where(x => x.DistrictId == districtId || x.DistrictId == null)
                             .Where(x => x.UserId == userId || x.UserId == null)
                             .Select(x => x.ReportEntityId)
                             .FirstOrDefaultAsync();
        }

        private async Task<UserReportAttribute> GetUserReportAttributeAsync(Guid reportEntityId,
                                                                            Guid userId,
                                                                            Guid districtId)
        {
            return await _dbContext.UserReportAttributes
                                   .FirstOrDefaultAsync(x => x.ReportEntityId == reportEntityId
                                                             && x.UserId == userId
                                                             && x.DistrictId == districtId);
        }

        protected override void DisposeManaged()
        {
            _dbContext.Dispose();
        }
    }

    public static class UserReportAttributeServiceExtensions
    {
        public static int TrackReportExecutionSync(this IUserReportAttributeService service,
                                                   Guid reportEntityId,
                                                   Guid userId,
                                                   Guid districtId)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            return AsyncHelper.RunSync(() => service.TrackReportExecution(reportEntityId, userId, districtId));
        }

        public static UserReportStats GetUserReportStatsSync(this IUserReportAttributeService service,
                                                             Guid reportEntityId,
                                                             Guid userId,
                                                             Guid districtId)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            return AsyncHelper.RunSync(() => service.GetUserReportStats(reportEntityId, userId, districtId));
        }

        public static void SetFavoriteSync(this IUserReportAttributeService service,
                                           Guid reportEntityId,
                                           Guid userId,
                                           Guid districtId)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            AsyncHelper.RunSync(() => service.SetFavorite(reportEntityId, userId, districtId));
        }

        public static void UnsetFavoriteSync(this IUserReportAttributeService service,
                                             Guid reportEntityId,
                                             Guid userId,
                                             Guid districtId)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            AsyncHelper.RunSync(() => service.UnsetFavorite(reportEntityId, userId, districtId));
        }

        public static Guid GetReportEntityIdSync(this IUserReportAttributeService service,
                                                 string reportName,
                                                 Guid userId,
                                                 Guid districtId)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            return AsyncHelper.RunSync(() => service.GetReportEntityId(reportName, userId, districtId));
        }
    }
}