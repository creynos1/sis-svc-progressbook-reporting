namespace ProgressBook.Reporting.Client.Models
{
    using System;

    public class ReportResult
    {
        public byte[] Content { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
    }
}