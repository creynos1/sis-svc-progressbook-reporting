namespace ProgressBook.Reporting.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using ProgressBook.Reporting.Data.Entities;
    using ProgressBook.Shared.Utilities.Data;

    public interface IPlaceRepository
    {
        IEnumerable<Place> GetAuthorizedPlaces(Guid userId);
    }

    public class PlaceRepository : AdoRepository<Place>, IPlaceRepository
    {
        public PlaceRepository()
            : base(new DbCommandExecutor(ConfigurationManager.ConnectionStrings["StudentInformation"].ConnectionString))
        {
        }

        public IEnumerable<Place> GetAuthorizedPlaces(Guid userId)
        {
            var sql = @"SELECT SchoolId, Code, SchoolName, DistrictId
FROM tblSchool AS s
WHERE s.IsActive = 1
    AND EXISTS
        (
            SELECT TOP 1 NULL
            FROM tblUserSecurityCache AS usc
            WHERE UserId = @UserId
                AND usc.SchoolID = s.SchoolId
                AND (CreateAccess = 1
                    OR ReadAccess = 1
                    OR UpdateAccess = 1
                    OR DeleteAccess = 1)
        )
    AND EXISTS
        (
            SELECT TOP 1 *
            FROM tblSchoolConfig AS sc
            WHERE sc.IsActive = 1
                AND sc.SchoolId = s.SchoolId
        )";

            var userIdParameter = new SqlParameter("UserId", SqlDbType.UniqueIdentifier)
                                  {
                                      Value = userId
                                  };
            return ReadAllRecords(sql, CommandType.Text, userIdParameter);
        }
    }
}