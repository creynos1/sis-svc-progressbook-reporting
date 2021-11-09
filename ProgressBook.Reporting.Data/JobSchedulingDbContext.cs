namespace ProgressBook.Reporting.Data
{
    using System.Data.Entity;
    using ProgressBook.Reporting.Data.Configuration;
    using ProgressBook.Reporting.Data.Entities;

    public class JobSchedulingDbContext : DbContext
    {
        static JobSchedulingDbContext()
        {
            Database.SetInitializer<JobSchedulingDbContext>(null);
        }

        public JobSchedulingDbContext(string nameOrConnectionString) :
            base(nameOrConnectionString)
        {
        }

        public virtual IDbSet<JobScheduling> JobSchedulings { get; set; }
        public virtual IDbSet<JobSchedulingResult> JobSchedulingResults { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new JobSchedulingConfiguration());
            modelBuilder.Configurations.Add(new JobSchedulingResultConfiguration());
        }
    }
}