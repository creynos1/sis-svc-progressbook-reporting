﻿

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
// TargetFrameworkVersion = 4.7
#pragma warning disable 1591    //  Ignore "Missing XML Comment" warning


namespace ProgressBook.Reporting.ExagoIntegration.VendorExtract
{

    #region Unit of work

    public interface IVendorExtractContext : System.IDisposable
    {
        System.Data.Entity.DbSet<User> Users { get; set; } // tblUser
        System.Data.Entity.DbSet<VendorExtractReportConfig> VendorExtractReportConfigs { get; set; } // tblVendorExtractReportConfig

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
    }

    #endregion

    #region Database context

    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.28.0.0")]
    public class VendorExtractContext : System.Data.Entity.DbContext, IVendorExtractContext
    {
        public System.Data.Entity.DbSet<User> Users { get; set; } // tblUser
        public System.Data.Entity.DbSet<VendorExtractReportConfig> VendorExtractReportConfigs { get; set; } // tblVendorExtractReportConfig

        static VendorExtractContext()
        {
            System.Data.Entity.Database.SetInitializer<VendorExtractContext>(null);
        }

        public VendorExtractContext()
            : base("Name=StudentInformation")
        {
        }

        public VendorExtractContext(string connectionString)
            : base(connectionString)
        {
        }

        public VendorExtractContext(string connectionString, System.Data.Entity.Infrastructure.DbCompiledModel model)
            : base(connectionString, model)
        {
        }

        public VendorExtractContext(System.Data.Common.DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }

        public VendorExtractContext(System.Data.Common.DbConnection existingConnection, System.Data.Entity.Infrastructure.DbCompiledModel model, bool contextOwnsConnection)
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

            modelBuilder.Configurations.Add(new UserConfiguration());
            modelBuilder.Configurations.Add(new VendorExtractReportConfigConfiguration());
        }

        public static System.Data.Entity.DbModelBuilder CreateModel(System.Data.Entity.DbModelBuilder modelBuilder, string schema)
        {
            modelBuilder.Configurations.Add(new UserConfiguration(schema));
            modelBuilder.Configurations.Add(new VendorExtractReportConfigConfiguration(schema));
            return modelBuilder;
        }
    }
    #endregion

    #region POCO classes

    // tblUser
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.28.0.0")]
    public class User
    {
        public System.Guid UserId { get; set; } // UserId (Primary key)
        public bool IsGroup { get; set; } // IsGroup
        public string FirstName { get; set; } // FirstName (length: 50)
        public string LastName { get; set; } // LastName (length: 100)
        public int? EmployeeNumber { get; set; } // EmployeeNumber
        public string Domain { get; set; } // Domain (length: 32)
        public string Username { get; set; } // Username (length: 64)
        public string EmailAddress { get; set; } // EmailAddress (length: 250)

        ///<summary>
        /// Value is not recursive and can be any level.  For Groups it defines the target or context school.  For Users it defines the default school.
        ///</summary>
        public System.Guid SchoolId { get; set; } // SchoolId

        ///<summary>
        /// Value is not recursive and can be any level.  This field defines the School that controls this record.
        ///</summary>
        public System.Guid? AdministrativeSchoolId { get; set; } // AdministrativeSchoolId
        public bool IsPrivileged { get; set; } // IsPrivileged
        public bool IsSecurityFaulted { get; set; } // IsSecurityFaulted
        public bool LegalNotice { get; set; } // LegalNotice

        ///<summary>
        /// Bit-masked Staff Job Functions.
        ///</summary>
        public int MasterRoles { get; set; } // MasterRoles
        public byte IsFixed { get; set; } // IsFixed
        public bool IsActive { get; set; } // IsActive
        public byte[] Rv { get; private set; } // rv (length: 8)
        public System.DateTime DateModified { get; set; } // DateModified
        public int? OrigId { get; set; } // _OrigId
        public bool IsEnabled { get; set; } // IsEnabled
        public bool IsSessionReuseEnabled { get; set; } // IsSessionReuseEnabled
        public System.Guid? CentralUid { get; set; } // CentralUID
        public byte UserTypeId { get; set; } // UserTypeId

        public User()
        {
            UserId = System.Guid.NewGuid();
            IsGroup = false;
            IsPrivileged = false;
            IsSecurityFaulted = false;
            LegalNotice = false;
            MasterRoles = 1;
            IsFixed = 0;
            IsActive = true;
            DateModified = System.DateTime.Now;
            IsEnabled = true;
            IsSessionReuseEnabled = false;
            UserTypeId = 1;
        }
    }

    // tblVendorExtractReportConfig
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.28.0.0")]
    public class VendorExtractReportConfig
    {
        public System.Guid VendorExtractReportConfigId { get; set; } // VendorExtractReportConfigId (Primary key)
        public System.Guid ReportEntityId { get; set; } // ReportEntityId
        public System.Guid DistrictId { get; set; } // DistrictId
        public string Config { get; set; } // Config
    }

    #endregion

    #region POCO Configuration

    // tblUser
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.28.0.0")]
    public class UserConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<User>
    {
        public UserConfiguration()
            : this("dbo")
        {
        }

        public UserConfiguration(string schema)
        {
            ToTable("tblUser", schema);
            HasKey(x => x.UserId);

            Property(x => x.UserId).HasColumnName(@"UserId").HasColumnType("uniqueidentifier").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.IsGroup).HasColumnName(@"IsGroup").HasColumnType("bit").IsRequired();
            Property(x => x.FirstName).HasColumnName(@"FirstName").HasColumnType("varchar").IsOptional().IsUnicode(false).HasMaxLength(50);
            Property(x => x.LastName).HasColumnName(@"LastName").HasColumnType("varchar").IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.EmployeeNumber).HasColumnName(@"EmployeeNumber").HasColumnType("int").IsOptional();
            Property(x => x.Domain).HasColumnName(@"Domain").HasColumnType("varchar").IsOptional().IsUnicode(false).HasMaxLength(32);
            Property(x => x.Username).HasColumnName(@"Username").HasColumnType("varchar").IsOptional().IsUnicode(false).HasMaxLength(64);
            Property(x => x.EmailAddress).HasColumnName(@"EmailAddress").HasColumnType("varchar").IsOptional().IsUnicode(false).HasMaxLength(250);
            Property(x => x.SchoolId).HasColumnName(@"SchoolId").HasColumnType("uniqueidentifier").IsRequired();
            Property(x => x.AdministrativeSchoolId).HasColumnName(@"AdministrativeSchoolId").HasColumnType("uniqueidentifier").IsOptional();
            Property(x => x.IsPrivileged).HasColumnName(@"IsPrivileged").HasColumnType("bit").IsRequired();
            Property(x => x.IsSecurityFaulted).HasColumnName(@"IsSecurityFaulted").HasColumnType("bit").IsRequired();
            Property(x => x.LegalNotice).HasColumnName(@"LegalNotice").HasColumnType("bit").IsRequired();
            Property(x => x.MasterRoles).HasColumnName(@"MasterRoles").HasColumnType("int").IsRequired();
            Property(x => x.IsFixed).HasColumnName(@"IsFixed").HasColumnType("tinyint").IsRequired();
            Property(x => x.IsActive).HasColumnName(@"IsActive").HasColumnType("bit").IsRequired();
            Property(x => x.Rv).HasColumnName(@"rv").HasColumnType("timestamp").IsRequired().IsFixedLength().HasMaxLength(8).IsRowVersion().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Computed);
            Property(x => x.DateModified).HasColumnName(@"DateModified").HasColumnType("smalldatetime").IsRequired();
            Property(x => x.OrigId).HasColumnName(@"_OrigId").HasColumnType("int").IsOptional();
            Property(x => x.IsEnabled).HasColumnName(@"IsEnabled").HasColumnType("bit").IsRequired();
            Property(x => x.IsSessionReuseEnabled).HasColumnName(@"IsSessionReuseEnabled").HasColumnType("bit").IsRequired();
            Property(x => x.CentralUid).HasColumnName(@"CentralUID").HasColumnType("uniqueidentifier").IsOptional();
            Property(x => x.UserTypeId).HasColumnName(@"UserTypeId").HasColumnType("tinyint").IsRequired();
        }
    }

    // tblVendorExtractReportConfig
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.28.0.0")]
    public class VendorExtractReportConfigConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<VendorExtractReportConfig>
    {
        public VendorExtractReportConfigConfiguration()
            : this("dbo")
        {
        }

        public VendorExtractReportConfigConfiguration(string schema)
        {
            ToTable("tblVendorExtractReportConfig", schema);
            HasKey(x => x.VendorExtractReportConfigId);

            Property(x => x.VendorExtractReportConfigId).HasColumnName(@"VendorExtractReportConfigId").HasColumnType("uniqueidentifier").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.ReportEntityId).HasColumnName(@"ReportEntityId").HasColumnType("uniqueidentifier").IsRequired();
            Property(x => x.DistrictId).HasColumnName(@"DistrictId").HasColumnType("uniqueidentifier").IsRequired();
            Property(x => x.Config).HasColumnName(@"Config").HasColumnType("varchar(max)").IsRequired().IsUnicode(false);
        }
    }

    #endregion

}
// </auto-generated>

