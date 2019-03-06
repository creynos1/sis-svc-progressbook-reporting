namespace ProgressBook.Reporting.Data
{
    using System;
    using System.Data.Entity;
    using System.Linq.Expressions;
    using ProgressBook.Reporting.Data.Configuration;
    using ProgressBook.Reporting.Data.Entities;

    public interface IReportEntityDbContext : IDisposable
    {
        IDbSet<Report> Reports { get; set; }
        IDbSet<Folder> Folders { get; set; }
        IDbSet<Theme> Themes { get; set; }
        IDbSet<Template> Templates { get; set; }
        IDbSet<ReportEntity> ReportEntities { get; set; }
        IDbSet<TEntity> Set<TEntity>() where TEntity : class;
        int SaveChanges();
    }

    public class ReportEntityDbContext : DbContext, IReportEntityDbContext
    {
        private readonly Guid? _districtId;
        private readonly Guid? _userId;

        static ReportEntityDbContext()
        {
            Database.SetInitializer<ReportEntityDbContext>(null);
        }

        public ReportEntityDbContext(string nameOrConnectionString) :
            base(nameOrConnectionString)
        {
        }

        public ReportEntityDbContext(string nameOrConnectionString, Guid? districtId, Guid? userId) :
            base(nameOrConnectionString)
        {
            _districtId = districtId;
            _userId = userId;

            Expression<Func<ReportEntity, bool>> filter1 = (x =>
                                                                   (x.DistrictId == districtId || x.DistrictId == null) &&
                                                                   (x.UserId == userId || x.UserId == null)
                                                           );

            Expression<Func<Folder, bool>> filter2 = (x =>
                                                             (x.DistrictId == districtId || x.DistrictId == null) &&
                                                             (x.UserId == userId || x.UserId == null)
                                                     );

            Expression<Func<Report, bool>> filter3 = (x =>
                                                             (x.DistrictId == districtId || x.DistrictId == null) &&
                                                             (x.UserId == userId || x.UserId == null)
                                                     );

            Expression<Func<Template, bool>> filter4 = (x =>
                                                               (x.DistrictId == districtId || x.DistrictId == null) &&
                                                               (x.UserId == userId || x.UserId == null)
                                                       );

            Expression<Func<Theme, bool>> filter5 = (x =>
                                                            (x.DistrictId == districtId || x.DistrictId == null) &&
                                                            (x.UserId == userId || x.UserId == null)
                                                    );

            ReportEntities = new FilteredDbSet<ReportEntity>(this, filter1, EntityInit);
            Folders = new FilteredDbSet<Folder>(this, filter2, EntityInit);
            Reports = new FilteredDbSet<Report>(this, filter3, EntityInit);
            Templates = new FilteredDbSet<Template>(this, filter4, EntityInit);
            Themes = new FilteredDbSet<Theme>(this, filter5, EntityInit);
        }

        public virtual IDbSet<Report> Reports { get; set; }
        public virtual IDbSet<Folder> Folders { get; set; }
        public virtual IDbSet<Theme> Themes { get; set; }
        public virtual IDbSet<Template> Templates { get; set; }
        public virtual IDbSet<ReportEntity> ReportEntities { get; set; }

        IDbSet<TEntity> IReportEntityDbContext.Set<TEntity>()
        {
            if (typeof(TEntity) == typeof(Report))
            {
                return (IDbSet<TEntity>)Reports;
            }

            if (typeof(TEntity) == typeof(Folder))
            {
                return (IDbSet<TEntity>)Folders;
            }

            if (typeof(TEntity) == typeof(Theme))
            {
                return (IDbSet<TEntity>)Themes;
            }

            if (typeof(TEntity) == typeof(Template))
            {
                return (IDbSet<TEntity>)Templates;
            }

            if (typeof(TEntity) == typeof(ReportEntity))
            {
                return (IDbSet<TEntity>)ReportEntities;
            }

            return Set<TEntity>();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ReportEntityConfiguration());
            modelBuilder.Configurations.Add(new ReportConfiguration());
            modelBuilder.Configurations.Add(new FolderConfiguration());
            modelBuilder.Configurations.Add(new TemplateConfiguration());
            modelBuilder.Configurations.Add(new ThemeConfiguration());
        }

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries<ReportEntity>())
            {
                entry.Property("DateModified").CurrentValue = DateTime.Now;
            }

            return base.SaveChanges();
        }

        private void EntityInit(ReportEntity x)
        {
            x.UserId = _userId;
            x.DistrictId = _districtId;
        }
    }
}