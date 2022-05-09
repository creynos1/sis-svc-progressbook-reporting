using System;
using System.Data.Entity;
using ProgressBook.Reporting.Data.Configuration;
using ProgressBook.Reporting.Data.Entities;

namespace ProgressBook.Reporting.Data
{
    public class JobEntityDbContext : DbContext
    {
        private const string CONNECTION_STRING_NAME = "StudentInformation";
        static JobEntityDbContext()
        {
            Database.SetInitializer<JobEntityDbContext>(null);
        }
        public JobEntityDbContext(string nameOrConnectionString) :
            base(nameOrConnectionString)
        {
        }
        public JobEntityDbContext() :
            base(CONNECTION_STRING_NAME)
        {
        }
        public virtual IDbSet<JobEntity> JobEntities { get; set; }
        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries<JobEntity>())
            {
                entry.Property("DateModified").CurrentValue = DateTime.Now;
            }
            return base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new JobEntityConfiguration());
        }
    }

}