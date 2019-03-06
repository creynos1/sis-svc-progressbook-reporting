namespace ProgressBook.Reporting.Data.Entities
{
    using System;

    public class Place
    {
        public Guid DistrictId { get; set; }
        public Guid SchoolId { get; set; }
        public string Code { get; set; }
    }
}