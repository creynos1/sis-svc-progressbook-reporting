namespace ProgressBook.Reporting.Data.Entities
{
    using System;
    using System.Collections.Generic;

    public abstract class ReportEntity
    {
        protected ReportEntity(ReportEntityType reportEntityType)
        {
            Children = new List<ReportEntity>();
            ReportEntityType = reportEntityType;

            if (ReportEntityType == ReportEntityType.Report)
            {
                IsLeafNode = true;
            }
        }

        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? DistrictId { get; set; }
        public Guid? UserId { get; set; }
        public string Name { get; set; }
        public bool IsLeafNode { get; private set; }
        public bool IsReadOnly { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }
        public DateTime? DateModified { get; set; }
        public Guid? ModifiedBy { get; set; }
        public bool IsInternal { get; set; }
        public DateTime DateCreated { get; set; }
        
        public ReportEntityType ReportEntityType { get; private set; }
        public ReportEntity Parent { get; set; }
        public List<ReportEntity> Children { get; set; }

        public string Path
        {
            get
            {
                if (Parent != null)
                    return Parent.Path + "\\" + Name;

                return Name;
            }
        }
    }
}