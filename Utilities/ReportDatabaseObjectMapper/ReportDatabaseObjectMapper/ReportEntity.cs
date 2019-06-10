namespace ViewsExagoDependencies
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CoreReports.ReportEntity")]
    public partial class ReportEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ReportEntity()
        {
            ReportEntity1 = new HashSet<ReportEntity>();
        }

        public Guid ReportEntityId { get; set; }

        public Guid? ParentId { get; set; }

        public Guid? DistrictId { get; set; }

        public Guid? UserId { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        public byte[] BinaryContent { get; set; }

        public bool IsReadOnly { get; set; }

        public bool IsLeafNode { get; set; }

        public byte? EntityType { get; set; }

        public DateTime? DateModified { get; set; }

        public Guid? ModifiedBy { get; set; }

        public bool IsInternal { get; set; }

        public bool IsNestedPath { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReportEntity> ReportEntity1 { get; set; }

        public virtual ReportEntity ReportEntity2 { get; set; }
    }
}
