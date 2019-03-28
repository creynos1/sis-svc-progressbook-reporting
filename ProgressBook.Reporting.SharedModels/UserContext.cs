using System;

namespace ProgressBook.Reporting.SharedModels
{
    public class UserContext
    {
        public Guid DistrictId { get; set; }
        public Guid UserId { get; set; }
        public string DistrictIrn { get; set; }
        public Guid AuthorizationPlaceId { get; set; }
    }
}