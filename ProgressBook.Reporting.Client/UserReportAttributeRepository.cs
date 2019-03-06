namespace ProgressBook.Reporting.Client
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using ProgressBook.Reporting.Client.Data;

    public interface IUserReportAttributeRepository
    {
        Task<IEnumerable<UserReportAttribute>> GetUserReportAttributesAsync(Guid userId, Guid districtId);
    }

    public class UserReportAttributeRepository : IUserReportAttributeRepository
    {
        private readonly IAdHocReportsContext _dbContext;

        public UserReportAttributeRepository(IAdHocReportsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<UserReportAttribute>> GetUserReportAttributesAsync(Guid userId, Guid districtId)
        {
            var values = await _dbContext.UserReportAttributes
                                         .AsNoTracking()
                                         .Where(x => x.UserId == userId && x.DistrictId == districtId)
                                         .ToListAsync();
            return values;
        }
    }
}