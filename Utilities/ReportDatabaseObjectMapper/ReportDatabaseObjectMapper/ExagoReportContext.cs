namespace ViewsExagoDependencies
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ExagoReportContext : DbContext
    {
        public ExagoReportContext()
            : base("name=ExagoReportContext")
        {
        }

        public virtual DbSet<ReportEntity> ReportEntities { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReportEntity>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<ReportEntity>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<ReportEntity>()
                .Property(e => e.Content)
                .IsUnicode(false);

            modelBuilder.Entity<ReportEntity>()
                .HasMany(e => e.ReportEntity1)
                .WithOptional(e => e.ReportEntity2)
                .HasForeignKey(e => e.ParentId);
        }
    }
}
