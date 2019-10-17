namespace ProgressBook.Reporting.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ProgressBook.Reporting.Client.Models;
    using ProgressBook.Shared.Security;
    using ProgressBook.Shared.Security.Enums;
    using ProgressBook.Shared.Security.Infrastructure.Extensions;
    using ProgressBook.Shared.Security.Models;
    using ProgressBook.Shared.Security.Services;
    using ProgressBook.Shared.Utilities;
    using ProgressBook.Shared.Utilities.Threading.Tasks;

    public interface IAdHocReportService : IDisposable
    {
        Task<IEnumerable<AdHocReport>> GetAdHocReports(Guid userId,
                                                       Guid districtId,
                                                       Guid authorizationPlaceId,
                                                       bool includeStats = false);

        Task<IEnumerable<AdHocReport>> GetFavorites(Guid userId, Guid districtId, Guid authorizationPlaceId);
        Task<IEnumerable<AdHocReport>> GetRecent(Guid userId, Guid districtId, Guid authorizationPlaceId);

        Task<IEnumerable<AdHocReport>> GetVendorExtractReports(Guid userId,
                                                               Guid districtId,
                                                               Guid authorizationPlaceId,
                                                               bool includeStats = false);

        Task<IEnumerable<AdHocReport>> GetAdHocReports(Guid userId, Guid districtId, bool includeStats = false);
        Task<IEnumerable<AdHocReport>> GetFavorites(Guid userId, Guid districtId);
        Task<IEnumerable<AdHocReport>> GetRecent(Guid userId, Guid districtId);
        Task<IEnumerable<AdHocReport>> GetVendorExtractReports(Guid userId, Guid districtId, bool includeStats = false);
    }

    public class AdHocReportService : Disposable, IAdHocReportService
    {
        private readonly IAdHocReportsRepository _adHocReportsRepository;
        private readonly IUserAuthorizationService _authorizationService;
        private readonly IUserReportAttributeRepository _userReportAttributeRepository;

        public AdHocReportService(IUserAuthorizationService authorizationService,
                                  IUserReportAttributeRepository userReportAttributeRepository,
                                  IAdHocReportsRepository adHocReportsRepository)
        {
            _authorizationService = authorizationService;
            _userReportAttributeRepository = userReportAttributeRepository;
            _adHocReportsRepository = adHocReportsRepository;
        }

        public async Task<IEnumerable<AdHocReport>> GetAdHocReports(Guid userId,
                                                                    Guid districtId,
                                                                    Guid authorizationPlaceId,
                                                                    bool includeStats = false)
        {
            var resourceNamespace = Resources.AdHocReports.ReportFolders.Namespace;

            var reportTypeTree = await _authorizationService.GetUserResourceTree(userId,
                                                                                 authorizationPlaceId,
                                                                                 resourceNamespace);

            var resources = GetResourceList(reportTypeTree);

            var adHocReports =
                await _adHocReportsRepository.GetAdHocReports(userId, districtId, resources).ToListAsync();

            if (!includeStats)
            {
                return adHocReports;
            }

            var reportStats = await GetUserReportStatsAsync(userId, districtId);

            foreach (var report in adHocReports)
            {
                var key = GetReportKey(report.ReportEntityId, districtId, userId);
                report.UserReportStats = reportStats.ContainsKey(key) ? reportStats[key] : null;
            }

            return adHocReports;
        }

        public async Task<IEnumerable<AdHocReport>> GetFavorites(Guid userId, Guid districtId, Guid authorizationPlaceId)
        {
            var reports = await GetAdHocReports(userId, districtId, authorizationPlaceId, true);

            return reports.Where(x => x.UserReportStats != null && x.UserReportStats.IsFavorite)
                          .ToList();
        }

        public async Task<IEnumerable<AdHocReport>> GetRecent(Guid userId, Guid districtId, Guid authorizationPlaceId)
        {
            var reports = await GetAdHocReports(userId, districtId, authorizationPlaceId, true);

            return reports
                .Where(x => x.UserReportStats != null && x.UserReportStats.LastRunDate.HasValue)
                .OrderByDescending(x => x.UserReportStats.LastRunDate.Value)
                .ToList();
        }

        public async Task<IEnumerable<AdHocReport>> GetVendorExtractReports(Guid userId,
                                                                            Guid districtId,
                                                                            Guid authorizationPlaceId,
                                                                            bool includeStats = false)
        {
            var resourceNamespace = Resources.AdHocReports.ReportFolders.Namespace;
            var reportTypeTree = await _authorizationService.GetUserResourceTree(userId,
                                                                                 authorizationPlaceId,
                                                                                 resourceNamespace);
            var resources = GetResourceList(reportTypeTree);

            var vendorExtractReports =
                await _adHocReportsRepository.GetVendorExtractReports(userId, districtId, resources).ToListAsync();

            if (!includeStats)
            {
                return vendorExtractReports;
            }

            var reportStats = await GetUserReportStatsAsync(userId, districtId);

            foreach (var report in vendorExtractReports)
            {
                var key = GetReportKey(report.ReportEntityId, districtId, userId);
                report.UserReportStats = reportStats.ContainsKey(key) ? reportStats[key] : null;
            }

            return vendorExtractReports.ToList();
        }

        public Task<IEnumerable<AdHocReport>> GetAdHocReports(Guid userId, Guid districtId, bool includeStats = false)
        {
            return GetAdHocReports(userId, districtId, districtId, includeStats);
        }

        public Task<IEnumerable<AdHocReport>> GetFavorites(Guid userId, Guid districtId)
        {
            return GetFavorites(userId, districtId, districtId);
        }

        public Task<IEnumerable<AdHocReport>> GetRecent(Guid userId, Guid districtId)
        {
            return GetRecent(userId, districtId, districtId);
        }

        public Task<IEnumerable<AdHocReport>> GetVendorExtractReports(Guid userId,
                                                                      Guid districtId,
                                                                      bool includeStats = false)
        {
            return GetVendorExtractReports(userId, districtId, districtId, includeStats);
        }

        private async Task<Dictionary<string, UserReportStats>> GetUserReportStatsAsync(Guid userId, Guid districtId)
        {
            var userReportAttributes = await _userReportAttributeRepository.GetUserReportAttributesAsync(userId,
                                                                                                         districtId);
            return userReportAttributes
                .ToDictionary(
                    key => GetReportKey(key.ReportEntityId, key.DistrictId, key.UserId),
                    attr => new UserReportStats
                            {
                                IsFavorite = attr.IsFavorite,
                                RunCount = attr.RunCount,
                                LastRunDate = attr.LastRunDate
                            });
        }

        private IEnumerable<ResourceTree> GetResourceList(ResourceTree tree)
        {
            var list = new List<ResourceTree>();
            list.AddRange(tree.Children.Where(x => x.ResourceActivities.Select(y => y.Activity).Contains(Activity.View)));

            if (tree.Children == null || !tree.Children.Any())
            {
                return list;
            }

            foreach (var subtree in tree.Children)
            {
                list.AddRange(GetResourceList(subtree));
            }

            return list;
        }

        private static string GetReportKey(Guid reportEntityId, Guid districtId, Guid userId)
        {
            return $"{reportEntityId}_{districtId}_{userId}";
        }

        protected override void DisposeManaged()
        {
            _authorizationService.Dispose();
        }
    }

    public static class AdHocReportServiceExtensions
    {
        public static IEnumerable<AdHocReport> GetAdHocReportsSync(this IAdHocReportService service,
                                                                   Guid userId,
                                                                   Guid districtId,
                                                                   Guid authorizationPlaceId,
                                                                   bool includeStats = false)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            return
                AsyncHelper.RunSync(
                    () => service.GetAdHocReports(userId, districtId, authorizationPlaceId, includeStats));
        }

        public static IEnumerable<AdHocReport> GetFavoritesSync(this IAdHocReportService service,
                                                                Guid userId,
                                                                Guid districtId,
                                                                Guid authorizationPlaceId)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }
            return AsyncHelper.RunSync(() => service.GetFavorites(userId, districtId, authorizationPlaceId));
        }

        public static IEnumerable<AdHocReport> GetRecentSync(this IAdHocReportService service,
                                                             Guid userId,
                                                             Guid districtId,
                                                             Guid authorizationPlaceId)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }
            return AsyncHelper.RunSync(() => service.GetRecent(userId, districtId, authorizationPlaceId));
        }

        public static IEnumerable<AdHocReport> GetVendorExtractReportsSync(this IAdHocReportService service,
                                                                           Guid userId,
                                                                           Guid districtId,
                                                                           Guid authorizationPlaceId,
                                                                           bool includeStats = false)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }
            return AsyncHelper.RunSync(() => service.GetVendorExtractReports(userId, districtId, authorizationPlaceId, includeStats));
        }

        public static IEnumerable<AdHocReport> GetAdHocReportsSync(this IAdHocReportService service,
                                                                   Guid userId,
                                                                   Guid districtId,
                                                                   bool includeStats = false)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            return AsyncHelper.RunSync(() => service.GetAdHocReports(userId, districtId, includeStats));
        }

        public static IEnumerable<AdHocReport> GetFavoritesSync(this IAdHocReportService service,
                                                                Guid userId,
                                                                Guid districtId)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }
            return AsyncHelper.RunSync(() => service.GetFavorites(userId, districtId));
        }

        public static IEnumerable<AdHocReport> GetRecentSync(this IAdHocReportService service,
                                                             Guid userId,
                                                             Guid districtId)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }
            return AsyncHelper.RunSync(() => service.GetRecent(userId, districtId));
        }

        public static IEnumerable<AdHocReport> GetVendorExtractReportsSync(this IAdHocReportService service,
                                                                           Guid userId,
                                                                           Guid districtId,
                                                                           bool includeStats = false)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }
            return AsyncHelper.RunSync(() => service.GetVendorExtractReports(userId, districtId, includeStats));
        }
    }
}