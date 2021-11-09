namespace ProgressBook.Reporting.Client.Tests
{
    using System;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Linq;
    using ProgressBook.Shared.Security;
    using ProgressBook.Shared.Security.Enums;
    using ProgressBook.Shared.Security.Models;
    using ProgressBook.Shared.Security.Services;

    public class TestHelper
    {
        public static Guid InsertTestRole(Guid adminSchoolId, string roleName)
        {
            var roleId = Guid.NewGuid();
            var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["StudentInformation"].ConnectionString);
            var cmd = conn.CreateCommand();

            try
            {
                conn.Open();

                cmd.CommandText = "INSERT INTO tblRole (RoleId, AdministrativeSchool, Named, IsPrivileged, DateModified, IsFixed)" +
                                  "VALUES (@RoleId, @AdministrativeSchoolId, @RoleName, 0, GETDATE(), 0)";
                cmd.Parameters.Add(new SqlParameter {ParameterName = "@RoleId", Value = roleId});
                cmd.Parameters.Add(new SqlParameter {ParameterName = "@AdministrativeSchoolId", Value = adminSchoolId});
                cmd.Parameters.Add(new SqlParameter {ParameterName = "@RoleName", Value = roleName});
                cmd.ExecuteNonQuery();

                return roleId;
            }
            finally
            {
                cmd.Dispose();
                conn.Dispose();
            }
        }

        public static void InsertTestRoleAssignment(Guid userId, Guid placeId, Guid roleId)
        {
            var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["StudentInformation"].ConnectionString);
            var cmd = conn.CreateCommand();

            try
            {
                conn.Open();
                cmd.CommandText = "INSERT INTO tblUserSecurityTemplate (UserId, SchoolId, RoleId)" +
                                  "VALUES (@UserId, @SchoolId, @RoleId)";
                cmd.Parameters.Add(new SqlParameter {ParameterName = "@UserId", Value = userId});
                cmd.Parameters.Add(new SqlParameter {ParameterName = "@SchoolId", Value = placeId});
                cmd.Parameters.Add(new SqlParameter {ParameterName = "@RoleId", Value = roleId});
                cmd.ExecuteNonQuery();
            }
            finally
            {
                cmd.Dispose();
                conn.Dispose();
            }
        }

        public static Guid InsertTestUser(string username, Guid placeId)
        {
            var userId = Guid.NewGuid();
            var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["StudentInformation"].ConnectionString);
            var cmd = conn.CreateCommand();

            try
            {
                conn.Open();

                cmd.CommandText = "INSERT INTO [Security].[MasterUser] (MasterUserId) VALUES (@UserId)\r\n" +
                                  "INSERT INTO tblUser (UserId, LastName, Username, AdministrativeSchoolId, SchoolId, DateModified, UserTypeId)" +
                                  "VALUES (@UserId, @UserName, @UserName, @SchoolId, @SchoolId, GETDATE(), 1)\r\n" +
                                  "INSERT INTO tblUserTree (ParentId, ChildId) VALUES (@UserId, @UserId)";
                cmd.Parameters.Add(new SqlParameter {ParameterName = "@UserId", Value = userId});
                cmd.Parameters.Add(new SqlParameter {ParameterName = "@SchoolId", Value = placeId});
                cmd.Parameters.Add(new SqlParameter {ParameterName = "@Username", Value = username});
                cmd.ExecuteNonQuery();

                return userId;
            }
            finally
            {
                cmd.Dispose();
                conn.Dispose();
            }
        }

        public static Guid InsertTestFolder(Guid placeId, Guid userId, string name)
        {
            var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["StudentInformation"].ConnectionString);
            var cmd = conn.CreateCommand();

            try
            {
                conn.Open();
                cmd.CommandText = "DECLARE @tmp table (id uniqueidentifier)\r\n" +
                                  "INSERT INTO [CoreReports].ReportEntity (DistrictId, UserId, Name, EntityType)\r\n" +
                                  "OUTPUT inserted.ReportEntityId into @tmp (id)\r\n" +
                                  "VALUES (@PlaceId, @UserId, @Name, 0)\r\n" +
                                  "SELECT id FROM @tmp";
                cmd.Parameters.Add(new SqlParameter { ParameterName = "@UserId", Value = userId });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "@PlaceId", Value = placeId });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "@Name", Value = name });
                return (Guid)cmd.ExecuteScalar();
            }
            finally
            {
                cmd.Dispose();
                conn.Dispose();
            }
        }

        public static void InsertTestReport(Guid placeId, Guid userId, Guid parentId, string name, string content)
        {
            var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["StudentInformation"].ConnectionString);
            var cmd = conn.CreateCommand();

            try
            {
                conn.Open();
                cmd.CommandText = "INSERT INTO [CoreReports].ReportEntity (ParentId, DistrictId, UserId, Name, Content, EntityType)" +
                                  "VALUES (@ParentId, @PlaceId, @UserId, @Name, @Content, 1)";
                cmd.Parameters.Add(new SqlParameter { ParameterName = "@ParentId", Value = parentId });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "@UserId", Value = userId });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "@PlaceId", Value = placeId });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "@Name", Value = name });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "@Content", Value = content});
                cmd.ExecuteNonQuery();
            }
            finally
            {
                cmd.Dispose();
                conn.Dispose();
            }
        }

        public static void GrantAccessAllReportTypes(Guid userId, Guid placeId)
        {
            IResourceTreeService resourceTreeService = null;
            IRolePermissionService rolePermissionService = null;

            try
            {
                rolePermissionService = Shared.Security.Factories.DefaultRolePermissionServiceFactory.Instance.Create();
                resourceTreeService = Shared.Security.Factories.DefaultResourceTreeServiceFactory.Instance.Create();

                var roleId = InsertTestRole(placeId, "Test - All Report Types " + new Random((int)DateTime.Now.Ticks).Next());
                var studentReports = resourceTreeService.GetResourceTreeSync(Resources.AdHocReports.ReportFolders.Namespace);
                rolePermissionService.SetPermissionsSync(roleId,
                    new[]
                    {
                        new GrantedPermission {ResourceActivityId = studentReports.ResourceActivities.FirstOrDefault(x => x.Activity == Activity.View).ResourceActivityId, IsAllowed = true}
                    });

                InsertTestRoleAssignment(userId, placeId, roleId);
            }
            finally
            {
                rolePermissionService.Dispose();
                resourceTreeService.Dispose();
            }
        }

        public static Guid InsertTestSchool(Guid districtId, string name, string irn = null)
        {
            Guid schoolId = Guid.NewGuid();

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["StudentInformation"].ConnectionString))
            {
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                INSERT INTO tblSchool (SchoolId, SchoolName, DistrictId, DateModified, SchoolType, IsFixed, IsActive, IRN)
                VALUES (@SchoolId, @SchoolName, @DistrictId, @DateModified, 1, 1, 1, @IRN)";
                cmd.Parameters.AddWithValue("@SchoolId", schoolId);
                cmd.Parameters.AddWithValue("@DistrictId", districtId);
                cmd.Parameters.AddWithValue("@SchoolName", name);
                cmd.Parameters.AddWithValue("@DateModified", DateTime.Now);
                if (!string.IsNullOrEmpty(irn))
                {
                    cmd.Parameters.AddWithValue("@IRN", irn);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IRN", DBNull.Value);
                }

                cmd.ExecuteNonQuery();

                cmd = connection.CreateCommand();
                cmd.CommandText = @"
                INSERT INTO tblSchoolTree (ParentId, ChildId)
                VALUES (@DistrictId, @SchoolId)";
                cmd.Parameters.AddWithValue("@SchoolId", schoolId);
                cmd.Parameters.AddWithValue("@DistrictId", districtId);
                cmd.ExecuteNonQuery();

                cmd = connection.CreateCommand();
                cmd.CommandText = @"
                INSERT INTO tblSchoolTree (ParentId, ChildId)
                VALUES (@SchoolId, @SchoolId)";
                cmd.Parameters.AddWithValue("@SchoolId", schoolId);
                cmd.ExecuteNonQuery();

                // copy ethnicity codes
                cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO tblCode (CodeTypeId, SchoolId, Code, Name)
                    SELECT DISTINCT 1, @SchoolId, c.Code, c.Code
                    FROM tblCode c
                        INNER JOIN tblSchool s ON c.SchoolId = s.SchoolId
                    WHERE
                        c.CodeTypeId = 1
                    AND s.IsDistrict = 1
                    AND c.IsActive = 1";
                cmd.Parameters.AddWithValue("@SchoolId", schoolId);
                cmd.ExecuteNonQuery();
                connection.Close();

                return schoolId;
            }
        }

        public static Guid InsertTestDistrict(string name)
        {
            Guid districtId = Guid.NewGuid();

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["StudentInformation"].ConnectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                INSERT INTO tblSchool (SchoolId, SchoolName, DateModified, SchoolType, IsFixed, IsActive)
                VALUES (@SchoolId, @SchoolName, @DateModified, 2, 1, 1)";
                cmd.Parameters.AddWithValue("@SchoolId", districtId);
                cmd.Parameters.AddWithValue("@SchoolName", name);
                cmd.Parameters.AddWithValue("@DateModified", DateTime.Now);
                cmd.ExecuteNonQuery();

                cmd = connection.CreateCommand();
                cmd.CommandText = @"
                INSERT INTO tblSchoolTree (ParentId, ChildId)
                VALUES (@SchoolId, @SchoolId)";
                cmd.Parameters.AddWithValue("@SchoolId", districtId);
                cmd.ExecuteNonQuery();

                connection.Close();

                return districtId;
            }
        }
    }
}