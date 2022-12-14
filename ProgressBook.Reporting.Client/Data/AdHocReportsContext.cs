

// ------------------------------------------------------------------------------------------------
// This code was generated by EntityFramework Reverse POCO Generator (http://www.reversepoco.com/).
// Created by Simon Hughes (https://about.me/simon.hughes).
//
// Do not make changes directly to this file - edit the template instead.
//
// The following connection settings were used to generate this file:
//     Configuration file:     "ProgressBook.Reporting.Web\Web.config"
//     Connection String Name: "StudentInformation"
//     Connection String:      "Data Source=.;Initial Catalog=Current_sis_tccsa;User Id=pbadmin;password=**zapped**;"
// ------------------------------------------------------------------------------------------------
// Database Edition       : Developer Edition (64-bit)
// Database Engine Edition: Enterprise

// <auto-generated>
// ReSharper disable ConvertPropertyToExpressionBody
// ReSharper disable DoNotCallOverridableMethodsInConstructor
// ReSharper disable InconsistentNaming
// ReSharper disable PartialMethodWithSinglePart
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantNameQualifier
// ReSharper disable RedundantOverridenMember
// ReSharper disable UseNameofExpression
// TargetFrameworkVersion = 4.8
#pragma warning disable 1591    //  Ignore "Missing XML Comment" warning


namespace ProgressBook.Reporting.Client.Data
{
    using System.Linq;

    #region Unit of work

    public interface IAdHocReportsContext : System.IDisposable
    {
        System.Data.Entity.DbSet<ReportEntityInfo> ReportEntityInfo { get; set; } // ReportEntityInfo
        System.Data.Entity.DbSet<UserReportAttribute> UserReportAttributes { get; set; } // UserReportAttribute

        int SaveChanges();
        System.Threading.Tasks.Task<int> SaveChangesAsync();
        System.Threading.Tasks.Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken);
        System.Data.Entity.Infrastructure.DbChangeTracker ChangeTracker { get; }
        System.Data.Entity.Infrastructure.DbContextConfiguration Configuration { get; }
        System.Data.Entity.Database Database { get; }
        System.Data.Entity.Infrastructure.DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        System.Data.Entity.Infrastructure.DbEntityEntry Entry(object entity);
        System.Collections.Generic.IEnumerable<System.Data.Entity.Validation.DbEntityValidationResult> GetValidationErrors();
        System.Data.Entity.DbSet Set(System.Type entityType);
        System.Data.Entity.DbSet<TEntity> Set<TEntity>() where TEntity : class;
        string ToString();

        // Stored Procedures
        System.Collections.Generic.List<ProcNcaaEligibilityDivIReturnModel> ProcNcaaEligibilityDivI(string districtId, string schoolYear, string schoolName, int? studentNumber);
        System.Collections.Generic.List<ProcNcaaEligibilityDivIReturnModel> ProcNcaaEligibilityDivI(string districtId, string schoolYear, string schoolName, int? studentNumber, out int procResult);
        System.Threading.Tasks.Task<System.Collections.Generic.List<ProcNcaaEligibilityDivIReturnModel>> ProcNcaaEligibilityDivIAsync(string districtId, string schoolYear, string schoolName, int? studentNumber);

        System.Collections.Generic.List<ProcNcaaEligibilityDivIiReturnModel> ProcNcaaEligibilityDivIi(string districtId, string schoolYear, string schoolName, int? studentNumber);
        System.Collections.Generic.List<ProcNcaaEligibilityDivIiReturnModel> ProcNcaaEligibilityDivIi(string districtId, string schoolYear, string schoolName, int? studentNumber, out int procResult);
        System.Threading.Tasks.Task<System.Collections.Generic.List<ProcNcaaEligibilityDivIiReturnModel>> ProcNcaaEligibilityDivIiAsync(string districtId, string schoolYear, string schoolName, int? studentNumber);

    }

    #endregion

    #region Database context

    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.29.1.0")]
    public class AdHocReportsContext : System.Data.Entity.DbContext, IAdHocReportsContext
    {
        public System.Data.Entity.DbSet<ReportEntityInfo> ReportEntityInfo { get; set; } // ReportEntityInfo
        public System.Data.Entity.DbSet<UserReportAttribute> UserReportAttributes { get; set; } // UserReportAttribute

        static AdHocReportsContext()
        {
            System.Data.Entity.Database.SetInitializer<AdHocReportsContext>(null);
        }

        public AdHocReportsContext()
            : base("Name=StudentInformation")
        {
        }

        public AdHocReportsContext(string connectionString)
            : base(connectionString)
        {
        }

        public AdHocReportsContext(string connectionString, System.Data.Entity.Infrastructure.DbCompiledModel model)
            : base(connectionString, model)
        {
        }

        public AdHocReportsContext(System.Data.Common.DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }

        public AdHocReportsContext(System.Data.Common.DbConnection existingConnection, System.Data.Entity.Infrastructure.DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public bool IsSqlParameterNull(System.Data.SqlClient.SqlParameter param)
        {
            var sqlValue = param.SqlValue;
            var nullableValue = sqlValue as System.Data.SqlTypes.INullable;
            if (nullableValue != null)
                return nullableValue.IsNull;
            return (sqlValue == null || sqlValue == System.DBNull.Value);
        }

        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new ReportEntityInfoConfiguration());
            modelBuilder.Configurations.Add(new UserReportAttributeConfiguration());
        }

        public static System.Data.Entity.DbModelBuilder CreateModel(System.Data.Entity.DbModelBuilder modelBuilder, string schema)
        {
            modelBuilder.Configurations.Add(new ReportEntityInfoConfiguration(schema));
            modelBuilder.Configurations.Add(new UserReportAttributeConfiguration(schema));
            return modelBuilder;
        }

        // Stored Procedures
        public System.Collections.Generic.List<ProcNcaaEligibilityDivIReturnModel> ProcNcaaEligibilityDivI(string districtId, string schoolYear, string schoolName, int? studentNumber)
        {
            int procResult;
            return ProcNcaaEligibilityDivI(districtId, schoolYear, schoolName, studentNumber, out procResult);
        }

        public System.Collections.Generic.List<ProcNcaaEligibilityDivIReturnModel> ProcNcaaEligibilityDivI(string districtId, string schoolYear, string schoolName, int? studentNumber, out int procResult)
        {
            var districtIdParam = new System.Data.SqlClient.SqlParameter { ParameterName = "@DistrictId", SqlDbType = System.Data.SqlDbType.VarChar, Direction = System.Data.ParameterDirection.Input, Value = districtId, Size = 36 };
            if (districtIdParam.Value == null)
                districtIdParam.Value = System.DBNull.Value;

            var schoolYearParam = new System.Data.SqlClient.SqlParameter { ParameterName = "@SchoolYear", SqlDbType = System.Data.SqlDbType.VarChar, Direction = System.Data.ParameterDirection.Input, Value = schoolYear, Size = 36 };
            if (schoolYearParam.Value == null)
                schoolYearParam.Value = System.DBNull.Value;

            var schoolNameParam = new System.Data.SqlClient.SqlParameter { ParameterName = "@SchoolName", SqlDbType = System.Data.SqlDbType.VarChar, Direction = System.Data.ParameterDirection.Input, Value = schoolName, Size = 100 };
            if (schoolNameParam.Value == null)
                schoolNameParam.Value = System.DBNull.Value;

            var studentNumberParam = new System.Data.SqlClient.SqlParameter { ParameterName = "@StudentNumber", SqlDbType = System.Data.SqlDbType.Int, Direction = System.Data.ParameterDirection.Input, Value = studentNumber.GetValueOrDefault(), Precision = 10, Scale = 0 };
            if (!studentNumber.HasValue)
                studentNumberParam.Value = System.DBNull.Value;

            var procResultParam = new System.Data.SqlClient.SqlParameter { ParameterName = "@procResult", SqlDbType = System.Data.SqlDbType.Int, Direction = System.Data.ParameterDirection.Output };
            var procResultData = Database.SqlQuery<ProcNcaaEligibilityDivIReturnModel>("EXEC @procResult = [CoreReports].[proc_NCAAEligibility_DivI] @DistrictId, @SchoolYear, @SchoolName, @StudentNumber", districtIdParam, schoolYearParam, schoolNameParam, studentNumberParam, procResultParam).ToList();

            procResult = (int) procResultParam.Value;
            return procResultData;
        }

        public async System.Threading.Tasks.Task<System.Collections.Generic.List<ProcNcaaEligibilityDivIReturnModel>> ProcNcaaEligibilityDivIAsync(string districtId, string schoolYear, string schoolName, int? studentNumber)
        {
            var districtIdParam = new System.Data.SqlClient.SqlParameter { ParameterName = "@DistrictId", SqlDbType = System.Data.SqlDbType.VarChar, Direction = System.Data.ParameterDirection.Input, Value = districtId, Size = 36 };
            if (districtIdParam.Value == null)
                districtIdParam.Value = System.DBNull.Value;

            var schoolYearParam = new System.Data.SqlClient.SqlParameter { ParameterName = "@SchoolYear", SqlDbType = System.Data.SqlDbType.VarChar, Direction = System.Data.ParameterDirection.Input, Value = schoolYear, Size = 36 };
            if (schoolYearParam.Value == null)
                schoolYearParam.Value = System.DBNull.Value;

            var schoolNameParam = new System.Data.SqlClient.SqlParameter { ParameterName = "@SchoolName", SqlDbType = System.Data.SqlDbType.VarChar, Direction = System.Data.ParameterDirection.Input, Value = schoolName, Size = 100 };
            if (schoolNameParam.Value == null)
                schoolNameParam.Value = System.DBNull.Value;

            var studentNumberParam = new System.Data.SqlClient.SqlParameter { ParameterName = "@StudentNumber", SqlDbType = System.Data.SqlDbType.Int, Direction = System.Data.ParameterDirection.Input, Value = studentNumber.GetValueOrDefault(), Precision = 10, Scale = 0 };
            if (!studentNumber.HasValue)
                studentNumberParam.Value = System.DBNull.Value;

            var procResultData = await Database.SqlQuery<ProcNcaaEligibilityDivIReturnModel>("EXEC [CoreReports].[proc_NCAAEligibility_DivI] @DistrictId, @SchoolYear, @SchoolName, @StudentNumber", districtIdParam, schoolYearParam, schoolNameParam, studentNumberParam).ToListAsync();

            return procResultData;
        }

        public System.Collections.Generic.List<ProcNcaaEligibilityDivIiReturnModel> ProcNcaaEligibilityDivIi(string districtId, string schoolYear, string schoolName, int? studentNumber)
        {
            int procResult;
            return ProcNcaaEligibilityDivIi(districtId, schoolYear, schoolName, studentNumber, out procResult);
        }

        public System.Collections.Generic.List<ProcNcaaEligibilityDivIiReturnModel> ProcNcaaEligibilityDivIi(string districtId, string schoolYear, string schoolName, int? studentNumber, out int procResult)
        {
            var districtIdParam = new System.Data.SqlClient.SqlParameter { ParameterName = "@DistrictId", SqlDbType = System.Data.SqlDbType.VarChar, Direction = System.Data.ParameterDirection.Input, Value = districtId, Size = 36 };
            if (districtIdParam.Value == null)
                districtIdParam.Value = System.DBNull.Value;

            var schoolYearParam = new System.Data.SqlClient.SqlParameter { ParameterName = "@SchoolYear", SqlDbType = System.Data.SqlDbType.VarChar, Direction = System.Data.ParameterDirection.Input, Value = schoolYear, Size = 36 };
            if (schoolYearParam.Value == null)
                schoolYearParam.Value = System.DBNull.Value;

            var schoolNameParam = new System.Data.SqlClient.SqlParameter { ParameterName = "@SchoolName", SqlDbType = System.Data.SqlDbType.VarChar, Direction = System.Data.ParameterDirection.Input, Value = schoolName, Size = 100 };
            if (schoolNameParam.Value == null)
                schoolNameParam.Value = System.DBNull.Value;

            var studentNumberParam = new System.Data.SqlClient.SqlParameter { ParameterName = "@StudentNumber", SqlDbType = System.Data.SqlDbType.Int, Direction = System.Data.ParameterDirection.Input, Value = studentNumber.GetValueOrDefault(), Precision = 10, Scale = 0 };
            if (!studentNumber.HasValue)
                studentNumberParam.Value = System.DBNull.Value;

            var procResultParam = new System.Data.SqlClient.SqlParameter { ParameterName = "@procResult", SqlDbType = System.Data.SqlDbType.Int, Direction = System.Data.ParameterDirection.Output };
            var procResultData = Database.SqlQuery<ProcNcaaEligibilityDivIiReturnModel>("EXEC @procResult = [CoreReports].[proc_NCAAEligibility_DivII] @DistrictId, @SchoolYear, @SchoolName, @StudentNumber", districtIdParam, schoolYearParam, schoolNameParam, studentNumberParam, procResultParam).ToList();

            procResult = (int) procResultParam.Value;
            return procResultData;
        }

        public async System.Threading.Tasks.Task<System.Collections.Generic.List<ProcNcaaEligibilityDivIiReturnModel>> ProcNcaaEligibilityDivIiAsync(string districtId, string schoolYear, string schoolName, int? studentNumber)
        {
            var districtIdParam = new System.Data.SqlClient.SqlParameter { ParameterName = "@DistrictId", SqlDbType = System.Data.SqlDbType.VarChar, Direction = System.Data.ParameterDirection.Input, Value = districtId, Size = 36 };
            if (districtIdParam.Value == null)
                districtIdParam.Value = System.DBNull.Value;

            var schoolYearParam = new System.Data.SqlClient.SqlParameter { ParameterName = "@SchoolYear", SqlDbType = System.Data.SqlDbType.VarChar, Direction = System.Data.ParameterDirection.Input, Value = schoolYear, Size = 36 };
            if (schoolYearParam.Value == null)
                schoolYearParam.Value = System.DBNull.Value;

            var schoolNameParam = new System.Data.SqlClient.SqlParameter { ParameterName = "@SchoolName", SqlDbType = System.Data.SqlDbType.VarChar, Direction = System.Data.ParameterDirection.Input, Value = schoolName, Size = 100 };
            if (schoolNameParam.Value == null)
                schoolNameParam.Value = System.DBNull.Value;

            var studentNumberParam = new System.Data.SqlClient.SqlParameter { ParameterName = "@StudentNumber", SqlDbType = System.Data.SqlDbType.Int, Direction = System.Data.ParameterDirection.Input, Value = studentNumber.GetValueOrDefault(), Precision = 10, Scale = 0 };
            if (!studentNumber.HasValue)
                studentNumberParam.Value = System.DBNull.Value;

            var procResultData = await Database.SqlQuery<ProcNcaaEligibilityDivIiReturnModel>("EXEC [CoreReports].[proc_NCAAEligibility_DivII] @DistrictId, @SchoolYear, @SchoolName, @StudentNumber", districtIdParam, schoolYearParam, schoolNameParam, studentNumberParam).ToListAsync();

            return procResultData;
        }

    }
    #endregion

    #region POCO classes

    // ReportEntityInfo
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.29.1.0")]
    public class ReportEntityInfo
    {
        public System.Guid ReportEntityId { get; set; } // ReportEntityId (Primary key)
        public System.Guid? ParentId { get; set; } // ParentId
        public string Path { get; set; } // Path (length: 1000)
        public string Name { get; set; } // Name (length: 255)
        public System.Guid? DistrictId { get; set; } // DistrictId
        public System.Guid? UserId { get; set; } // UserId
        public bool? NestedPath { get; set; } // NestedPath
        public bool? NestedDisplay { get; set; } // NestedDisplay
        public int IsSystemLevel { get; set; } // IsSystemLevel
        public int IsDistrictLevel { get; set; } // IsDistrictLevel
        public int IsUserLevel { get; set; } // IsUserLevel
    }

    // UserReportAttribute
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.29.1.0")]
    public class UserReportAttribute
    {
        public System.Guid ReportEntityId { get; set; } // ReportEntityId (Primary key)
        public System.Guid DistrictId { get; set; } // DistrictId (Primary key)
        public System.Guid UserId { get; set; } // UserId (Primary key)
        public System.DateTime? LastRunDate { get; set; } // LastRunDate
        public int RunCount { get; set; } // RunCount
        public bool IsFavorite { get; set; } // IsFavorite

        public UserReportAttribute()
        {
            RunCount = 0;
            IsFavorite = false;
        }
    }

    #endregion

    #region POCO Configuration

    // ReportEntityInfo
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.29.1.0")]
    public class ReportEntityInfoConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ReportEntityInfo>
    {
        public ReportEntityInfoConfiguration()
            : this("CoreReports")
        {
        }

        public ReportEntityInfoConfiguration(string schema)
        {
            ToTable("ReportEntityInfo", schema);
            HasKey(x => x.ReportEntityId);

            Property(x => x.ReportEntityId).HasColumnName(@"ReportEntityId").HasColumnType("uniqueidentifier").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.ParentId).HasColumnName(@"ParentId").HasColumnType("uniqueidentifier").IsOptional();
            Property(x => x.Path).HasColumnName(@"Path").HasColumnType("varchar").IsOptional().IsUnicode(false).HasMaxLength(1000);
            Property(x => x.Name).HasColumnName(@"Name").HasColumnType("varchar").IsOptional().IsUnicode(false).HasMaxLength(255);
            Property(x => x.DistrictId).HasColumnName(@"DistrictId").HasColumnType("uniqueidentifier").IsOptional();
            Property(x => x.UserId).HasColumnName(@"UserId").HasColumnType("uniqueidentifier").IsOptional();
            Property(x => x.NestedPath).HasColumnName(@"NestedPath").HasColumnType("bit").IsOptional();
            Property(x => x.NestedDisplay).HasColumnName(@"NestedDisplay").HasColumnType("bit").IsOptional();
            Property(x => x.IsSystemLevel).HasColumnName(@"IsSystemLevel").HasColumnType("int").IsRequired();
            Property(x => x.IsDistrictLevel).HasColumnName(@"IsDistrictLevel").HasColumnType("int").IsRequired();
            Property(x => x.IsUserLevel).HasColumnName(@"IsUserLevel").HasColumnType("int").IsRequired();
        }
    }

    // UserReportAttribute
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.29.1.0")]
    public class UserReportAttributeConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<UserReportAttribute>
    {
        public UserReportAttributeConfiguration()
            : this("CoreReports")
        {
        }

        public UserReportAttributeConfiguration(string schema)
        {
            ToTable("UserReportAttribute", schema);
            HasKey(x => new { x.ReportEntityId, x.DistrictId, x.UserId });

            Property(x => x.ReportEntityId).HasColumnName(@"ReportEntityId").HasColumnType("uniqueidentifier").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.DistrictId).HasColumnName(@"DistrictId").HasColumnType("uniqueidentifier").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.UserId).HasColumnName(@"UserId").HasColumnType("uniqueidentifier").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.LastRunDate).HasColumnName(@"LastRunDate").HasColumnType("datetime").IsOptional();
            Property(x => x.RunCount).HasColumnName(@"RunCount").HasColumnType("int").IsRequired();
            Property(x => x.IsFavorite).HasColumnName(@"IsFavorite").HasColumnType("bit").IsRequired();
        }
    }

    #endregion

    #region Stored procedure return models

    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.29.1.0")]
    public class ProcNcaaEligibilityDivIReturnModel
    {
        public System.Int32 SortOrder { get; set; }
        public System.String NCAASubjectArea { get; set; }
        public System.Int32? SubjectSortOrder { get; set; }
        public System.String CourseName { get; set; }
        public System.Decimal? Credit { get; set; }
        public System.String Grade { get; set; }
        public System.String QualityPoints { get; set; }
        public System.Int32? StudentNumber { get; set; }
        public System.String LastName { get; set; }
        public System.String FirstName { get; set; }
        public System.DateTime? Birthdate { get; set; }
        public System.String GradeLevel { get; set; }
        public System.Int16? Age { get; set; }
        public System.String Homeroom { get; set; }
        public System.String Program { get; set; }
        public System.String StatusCode { get; set; }
        public System.String SchoolName { get; set; }
        public System.String ACTScore { get; set; }
        public System.String SATScore { get; set; }
        public System.Guid DistrictId { get; set; }
        public System.Guid? SchoolId { get; set; }
        public System.Guid? SchoolYearId { get; set; }
        public System.String SchoolYear { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.29.1.0")]
    public class ProcNcaaEligibilityDivIiReturnModel
    {
        public System.Int32 SortOrder { get; set; }
        public System.String NCAASubjectArea { get; set; }
        public System.Int32? SubjectSortOrder { get; set; }
        public System.String CourseName { get; set; }
        public System.Decimal? Credit { get; set; }
        public System.String Grade { get; set; }
        public System.String QualityPoints { get; set; }
        public System.Int32? StudentNumber { get; set; }
        public System.String LastName { get; set; }
        public System.String FirstName { get; set; }
        public System.DateTime? Birthdate { get; set; }
        public System.String GradeLevel { get; set; }
        public System.Int16? Age { get; set; }
        public System.String Homeroom { get; set; }
        public System.String Program { get; set; }
        public System.String StatusCode { get; set; }
        public System.String SchoolName { get; set; }
        public System.String ACTScore { get; set; }
        public System.String SATScore { get; set; }
        public System.Guid DistrictId { get; set; }
        public System.Guid? SchoolId { get; set; }
        public System.Guid? SchoolYearId { get; set; }
        public System.String SchoolYear { get; set; }
    }

    #endregion

}
// </auto-generated>

