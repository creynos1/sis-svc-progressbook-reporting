using System.Linq;

namespace ProgressBook.Reporting.Client.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using ProgressBook.Reporting.SharedModels;
    using Xunit;

    public class AdHocReportServiceTests : IntegrationTests
    {
#if INTEGRATION_TESTS
        [Fact, AutoRollback]
        public void GetVendorExtractReports_ReturnTwo()
        {
            IAdHocReportService service = null;
            IUserReportAttributeService attributeService = null;

            try
            {
                // arrange
                var placeId = TestHelper.InsertTestDistrict("SomeTestDistrict1");
                var userId = TestHelper.InsertTestUser("sometestUser", placeId);
                service = AdHocReportServiceFactory.Instance.Create();
                attributeService = UserReportAttributeServiceFactory.Instance.Create();

                var folderId = TestHelper.InsertTestFolder(placeId, userId, "My Reports");
                TestHelper.InsertTestReport(placeId, userId, folderId, "Test1", "<report><main><description>test</description></main><custom_value><id>Is_Vendor_Extract</id><value>True</value></custom_value></report>");
                TestHelper.InsertTestReport(placeId, userId, folderId, "Test2", "<report><main><description>test</description></main><custom_value><id>Is_Vendor_Extract</id><value>True</value></custom_value></report>");
                TestHelper.InsertTestReport(placeId, userId, folderId, "Test3", "<report><main><description>test</description></main><custom_value><id>Is_Vendor_Extract</id><value>False</value></custom_value></report>");
                TestHelper.InsertTestReport(placeId, userId, folderId, "Test4", "<report><main><description>test</description></main></report>");
                attributeService.GetReportEntityId("My Reports\\Test1", userId, placeId);
                attributeService.GetReportEntityId("My Reports\\Test2", userId, placeId);
                attributeService.GetReportEntityId("My Reports\\Test3", userId, placeId);
                attributeService.GetReportEntityId("My Reports\\Test4", userId, placeId);

                // act
                var results = service.GetVendorExtractReportsSync(userId, placeId).ToList();

                // assert
                Assert.NotEmpty(results);
                Assert.Equal(2, results.Count);
            }
            finally
            {
                service.Dispose();
                attributeService.Dispose();
            }
        }


        [Fact, AutoRollback]
        public void GetAdHocReports_ShouldReturnReports_Student()
        {
            IResourceTreeService resourceTreeService = null;
            IAdHocReportService service = null;

            try
            {
                resourceTreeService = Security.Factories.DefaultResourceTreeServiceFactory.Instance.Create();

                // arrange
                var placeId = TestHelper.InsertTestDistrict("SomeTestDistrict1");
                var userId = TestHelper.InsertTestUser("sometestUser", placeId);
                service = AdHocReportServiceFactory.Instance.Create();

                var roleId = TestHelper.InsertTestRole(placeId, "SomeSuperAwesomeTestRole");
                var rolePermissionService = Security.Factories.DefaultRolePermissionServiceFactory.Instance.Create();

                var studentReports = resourceTreeService.GetResourceTreeSync(Resources.AdHocReports.ReportFolders.Student.Namespace);
                rolePermissionService.SetPermissionsSync(roleId,
                    new[]
                    {
                        new GrantedPermission {ResourceActivityId = studentReports.ResourceActivities.FirstOrDefault(x => x.Activity == Activity.View).ResourceActivityId, IsAllowed = true}
                    });

                TestHelper.InsertTestRoleAssignment(userId, placeId, roleId);

                // act
                var reports = service.GetAdHocReports(userId, placeId)
                    .OrderBy(x => x.Name)
                    .ToList();

                // assert
                Assert.NotEmpty(reports);

                Assert.Equal("Student\\Homeroom List with Counts", reports[1].Name);
            }
            finally
            {
                resourceTreeService?.Dispose();
                service?.Dispose();
            }
        }

        [Fact, AutoRollback]
        public void GetAdHocReports_ShouldReturnReports_EmisButNotEmisAdmin()
        {
            IResourceTreeService resourceTreeService = null;
            IAdHocReportService service = null;

            try
            {
                resourceTreeService = Security.Factories.DefaultResourceTreeServiceFactory.Instance.Create();

                // arrange
                var placeId = TestHelper.InsertTestDistrict("SomeTestDistrict1");
                var userId = TestHelper.InsertTestUser("sometestUser", placeId);
                service = AdHocReportServiceFactory.Instance.Create();

                var roleId = TestHelper.InsertTestRole(placeId, "SomeSuperAwesomeTestRole");
                var rolePermissionService = Security.Factories.DefaultRolePermissionServiceFactory.Instance.Create();

                var emisReports = resourceTreeService.GetResourceTreeSync(Resources.AdHocReports.ReportFolders.EMIS.Namespace);
                var emisAdminReports = resourceTreeService.GetResourceTreeSync(Resources.AdHocReports.ReportFolders.EMIS.Admin.Namespace);
                rolePermissionService.SetPermissionsSync(roleId,
                        new[]
                        {
                        new GrantedPermission { ResourceActivityId = emisReports.ResourceActivities.FirstOrDefault(x => x.Activity == Activity.View).ResourceActivityId, IsAllowed = true },
                        new GrantedPermission { ResourceActivityId = emisAdminReports.ResourceActivities.FirstOrDefault(x => x.Activity == Activity.View).ResourceActivityId, IsAllowed = false }
                        });

                TestHelper.InsertTestRoleAssignment(userId, placeId, roleId);

                // act
                var reports = service.GetAdHocReports(userId, placeId);

                // assert
                Assert.NotEmpty(reports);
            }
            finally
            {
                resourceTreeService?.Dispose();
                service?.Dispose();
            }
        }

        [Fact, AutoRollback]
        public void GetAdHocReports_ShouldReturnReports_MyReports()
        {
            IAdHocReportService service = null;

            try
            {
                // arrange
                var placeId = TestHelper.InsertTestDistrict("SomeTestDistrict1");
                var userId = TestHelper.InsertTestUser("sometestUser", placeId);
                service = AdHocReportServiceFactory.Instance.Create();

                var folderId = TestHelper.InsertTestFolder(placeId, userId, "My Reports");
                TestHelper.InsertTestReport(placeId, userId, folderId, "Test1234", "<report><main><description>test</description></main></report>");

                // act
                var reports = service.GetAdHocReports(userId, placeId).ToList();

                // assert
                Assert.NotEmpty(reports);
                Assert.Equal(1, reports.Count);

                Assert.Equal("My Reports\\Test1234", reports[0].Name);
                Assert.Equal("Test1234", reports[0].DisplayName);
            }
            finally
            {
                service.Dispose();
            }
        }

        [Fact, AutoRollback]
        public async void GetAdHocReportsAsync_ShouldReturnReports_MyReports()
        {
            IAdHocReportService service = null;

            try
            {
                // arrange
                var placeId = TestHelper.InsertTestDistrict("SomeTestDistrict1");
                var userId = TestHelper.InsertTestUser("sometestUser", placeId);
                service = AdHocReportServiceFactory.Instance.Create();

                var folderId = TestHelper.InsertTestFolder(placeId, userId, "My Reports");
                TestHelper.InsertTestReport(placeId, userId, folderId, "Test1234", "<report><main><description>test</description></main></report>");

                // act
                var reports = await service.GetAdHocReportsAsync(userId, placeId).ToListAsync();

                // assert
                Assert.NotEmpty(reports);
                Assert.Equal(1, reports.Count);

                Assert.Equal("My Reports\\Test1234", reports[0].Name);
                Assert.Equal("Test1234", reports[0].DisplayName);
            }
            finally
            {
                service.Dispose();
            }
        }

        [Fact, AutoRollback]
        public void GetRecent_ShouldReturn3()
        {
            IAdHocReportService service = null;
            IUserReportAttributeService attributeService = null;

            try
            {
                // arrange
                var placeId = TestHelper.InsertTestDistrict("SomeTestDistrict1");
                var userId = TestHelper.InsertTestUser("sometestUser", placeId);
                service = AdHocReportServiceFactory.Instance.Create();
                attributeService = UserReportAttributeServiceFactory.Instance.Create();

                var folderId = TestHelper.InsertTestFolder(placeId, userId, "My Reports");
                TestHelper.InsertTestReport(placeId, userId, folderId, "Test1", "<report><main><description>test</description></main></report>");
                TestHelper.InsertTestReport(placeId, userId, folderId, "Test2", "<report><main><description>test</description></main></report>");
                TestHelper.InsertTestReport(placeId, userId, folderId, "Test3", "<report><main><description>test</description></main></report>");
                attributeService.TrackReportExecution(attributeService.GetReportEntityId("My Reports\\Test1", userId, placeId), userId, placeId);
                attributeService.TrackReportExecution(attributeService.GetReportEntityId("My Reports\\Test2", userId, placeId), userId, placeId);
                attributeService.TrackReportExecution(attributeService.GetReportEntityId("My Reports\\Test3", userId, placeId), userId, placeId);

                // act
                var results = service.GetRecent(userId, placeId).ToList();

                // assert
                Assert.NotEmpty(results);
                Assert.Equal(3, results.Count);
            }
            finally
            {
                service.Dispose();
                attributeService.Dispose();
            }
        }

        [Fact, AutoRollback]
        public void GetFavorites_ShouldReturn4()
        {
            IAdHocReportService service = null;
            IUserReportAttributeService attributeService = null;

            try
            {
                // arrange
                var placeId = TestHelper.InsertTestDistrict("SomeTestDistrict1");
                var userId = TestHelper.InsertTestUser("sometestUser", placeId);
                service = AdHocReportServiceFactory.Instance.Create();
                attributeService = UserReportAttributeServiceFactory.Instance.Create();

                var folderId = TestHelper.InsertTestFolder(placeId, userId, "My Reports");
                TestHelper.InsertTestReport(placeId, userId, folderId, "Test1", "<report><main><description>test</description></main></report>");
                TestHelper.InsertTestReport(placeId, userId, folderId, "Test2", "<report><main><description>test</description></main></report>");
                TestHelper.InsertTestReport(placeId, userId, folderId, "Test3", "<report><main><description>test</description></main></report>");
                TestHelper.InsertTestReport(placeId, userId, folderId, "Test4", "<report><main><description>test</description></main></report>");
                attributeService.SetFavorite(attributeService.GetReportEntityId("My Reports\\Test1", userId, placeId), userId, placeId);
                attributeService.SetFavorite(attributeService.GetReportEntityId("My Reports\\Test2", userId, placeId), userId, placeId);
                attributeService.SetFavorite(attributeService.GetReportEntityId("My Reports\\Test3", userId, placeId), userId, placeId);
                attributeService.SetFavorite(attributeService.GetReportEntityId("My Reports\\Test4", userId, placeId), userId, placeId);

                // act
                var results = service.GetFavorites(userId, placeId).ToList();

                // assert
                Assert.NotEmpty(results);
                Assert.Equal(4, results.Count);
            }
            finally
            {
                service.Dispose();
                attributeService.Dispose();
            }
        }
		
#endif
    }

}
