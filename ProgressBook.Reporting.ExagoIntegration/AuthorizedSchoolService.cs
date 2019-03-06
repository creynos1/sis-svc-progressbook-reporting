namespace ProgressBook.Reporting.ExagoIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Caching;
    using ProgressBook.Reporting.Data.Entities;
    using ProgressBook.Reporting.Data.Repositories;
    using ProgressBook.Shared.Utilities;

    public interface IAuthorizedSchoolService
    {
        IEnumerable<string> GetAllowedSchoolCodes(Guid userId, Guid districtId);
        IEnumerable<Guid> GetAllowedSchoolIds(Guid userId, Guid districtId);
    }

    public class AuthorizedSchoolService : IAuthorizedSchoolService
    {
        private const string AuthorizedPlacesCacheKeyBase = "AuthorizedSchools";
        private readonly ObjectCache _cache = MemoryCache.Default;
        private readonly IPlaceRepository _placeRepository;

        public AuthorizedSchoolService(IPlaceRepository placeRepository)
        {
            _placeRepository = placeRepository;
        }

        public IEnumerable<string> GetAllowedSchoolCodes(Guid userId, Guid districtId)
        {
            return GetAuthorizedPlaces(userId, districtId).Select(p => p.Code).ToList();
        }

        public IEnumerable<Guid> GetAllowedSchoolIds(Guid userId, Guid districtId)
        {
            return GetAuthorizedPlaces(userId, districtId).Select(p => p.SchoolId).ToList();
        }

        private IEnumerable<Place> GetAuthorizedPlaces(Guid userId, Guid districtId)
        {
            return GetUserAuthorizedPlaces(userId)
                .Where(p => p.SchoolId == districtId
                            || p.DistrictId == districtId)
                .ToList();
        }

        private IEnumerable<Place> GetUserAuthorizedPlaces(Guid userId)
        {
            var userCacheKey = AuthorizedPlacesCacheKeyBase + userId;
            var places = _cache[userCacheKey] as IEnumerable<Place>;

            if (places == null)
            {
                places = _placeRepository.GetAuthorizedPlaces(userId);
                var absoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(60);
                _cache.Add(userCacheKey, places, absoluteExpiration);
            }
            return places;
        }
    }
}