namespace ProgressBook.Reporting.Client.Tests
{
#if INTEGRATION_TESTS
    public class UserReportAttributeServiceTests : IntegrationTests
    {
        [Fact, AutoRollback]
        public void TrackReportExecution_ShouldReturn1()
        {
            // arrange
            var placeId = TestHelper.InsertTestDistrict("SomeTestDistrict1");
            var userId = TestHelper.InsertTestUser("jollytestuser", placeId);

            using (var service = UserReportAttributeServiceFactory.Instance.Create())
            {
                var reportEntityId = service.GetReportEntityId("Student\\Homeroom List with Counts", userId, placeId);

                // act
                var result = service.TrackReportExecution(reportEntityId, userId, placeId);

                // assert
                Assert.Equal(1, result);
            }
        }

        [Fact, AutoRollback]
        public void TrackReportExecution_ShouldReturn3_AndNotChangeIsFavorite()
        {
            // arrange
            var placeId = TestHelper.InsertTestDistrict("SomeTestDistrict1");
            var userId = TestHelper.InsertTestUser("jollytestuser", placeId);

            using (var service = UserReportAttributeServiceFactory.Instance.Create())
            {
                var reportEntityId = service.GetReportEntityId("Student\\Homeroom List with Counts", userId, placeId);

                // act
                service.SetFavorite(reportEntityId, userId, placeId);
                service.TrackReportExecution(reportEntityId, userId, placeId);
                service.TrackReportExecution(reportEntityId, userId, placeId);
                var result = service.TrackReportExecution(reportEntityId, userId, placeId);

                // assert
                Assert.Equal(3, result);

                var stats = service.GetUserReportStats(reportEntityId, userId, placeId);
                Assert.True(stats.IsFavorite);
            }
        }

        [Fact, AutoRollback]
        public async void TrackReportExecutionAsync_ShouldReturn1()
        {
            // arrange
            var placeId = TestHelper.InsertTestDistrict("SomeTestDistrict1");
            var userId = TestHelper.InsertTestUser("jollytestuser", placeId);

            using (var service = UserReportAttributeServiceFactory.Instance.Create())
            {
                var reportEntityId = service.GetReportEntityId("Student\\Homeroom List with Counts", userId, placeId);

                // act
                var result = await service.TrackReportExecutionAsync(reportEntityId, userId, placeId);

                // assert
                Assert.Equal(1, result);
            }
        }

        [Fact, AutoRollback]
        public void TrackReportExecution_WhenReportNotFound_ShouldReturn0()
        {
            // arrange
            var placeId = TestHelper.InsertTestDistrict("SomeTestDistrict1");
            var userId = TestHelper.InsertTestUser("jollytestuser", placeId);

            using (var service = UserReportAttributeServiceFactory.Instance.Create())
            {
                var reportEntityId = service.GetReportEntityId("asdf;lkjasdf;lkasdjfa;lskjfas;lkj", userId, placeId);

                // act
                var result = service.TrackReportExecution(reportEntityId, userId, placeId);

                // assert
                Assert.Equal(0, result);
            }
        }


        [Fact, AutoRollback]
        public void TrackReportExecution_ShouldReturn0IfReportEntityIdIsInvalid()
        {
            // arrange
            var placeId = TestHelper.InsertTestDistrict("SomeTestDistrict1");
            var userId = TestHelper.InsertTestUser("jollytestuser", placeId);

            using (var service = UserReportAttributeServiceFactory.Instance.Create())
            {
                // act
                var result = service.TrackReportExecution(Guid.NewGuid(), userId, placeId);

                // assert
                Assert.Equal(0, result);
            }
        }

        [Fact, AutoRollback]
        public void TrackReportExecution_ShouldUpdateLastRunDate()
        {
            // arrange
            var placeId = TestHelper.InsertTestDistrict("SomeTestDistrict1");
            var userId = TestHelper.InsertTestUser("jollytestuser", placeId);

            using (var service = UserReportAttributeServiceFactory.Instance.Create())
            {
                // act
                var reportEntityId = service.GetReportEntityId("Student\\Homeroom List with Counts", userId, placeId);
                service.TrackReportExecution(reportEntityId, userId, placeId);

                // assert
                var dbContext = new AdHocReportsContext();
                var entity = dbContext.UserReportAttributes
                    .AsNoTracking()
                    .FirstOrDefault(x => x.ReportEntityId == reportEntityId && x.UserId == userId && x.DistrictId == placeId);

                Assert.NotNull(entity);
                Assert.NotNull(entity.LastRunDate);
                Assert.Equal(DateTime.Today, entity.LastRunDate.Value.Date);
            }
        }

        [Fact, AutoRollback]
        public void TrackReportExecution_ByReportEntityId_ShouldReturn4()
        {
            // arrange
            var placeId = TestHelper.InsertTestDistrict("SomeTestDistrict1");
            var userId = TestHelper.InsertTestUser("jollytestuser", placeId);

            using (var service = UserReportAttributeServiceFactory.Instance.Create())
            {
                var reportEntityId = service.GetReportEntityId("Student\\Homeroom List with Counts", userId, placeId);

                // act
                service.TrackReportExecution(reportEntityId, userId, placeId);
                service.TrackReportExecution(reportEntityId, userId, placeId);
                service.TrackReportExecution(reportEntityId, userId, placeId);
                var result = service.TrackReportExecution(reportEntityId, userId, placeId);

                // assert
                Assert.Equal(4, result);
            }
        }

        [Fact, AutoRollback]
        public void GetUserReportStats()
        {
            // arrange
            var placeId = TestHelper.InsertTestDistrict("SomeTestDistrict1");
            var userId = TestHelper.InsertTestUser("jollytestuser", placeId);

            using (var service = UserReportAttributeServiceFactory.Instance.Create())
            {
                var reportEntityId = service.GetReportEntityId("Student\\Homeroom List with Counts", userId, placeId);

                service.TrackReportExecution(reportEntityId, userId, placeId);
                service.TrackReportExecution(reportEntityId, userId, placeId);

                // act
                var result = service.GetUserReportStats(reportEntityId, userId, placeId);

                // assert
                Assert.NotNull(result);
                Assert.Equal(DateTime.Today, result.LastRunDate.Value.Date);
                Assert.Equal(2, result.RunCount);
                Assert.False(result.IsFavorite);
            }
        }

        [Fact, AutoRollback]
        public void GetUserReportStats_WhenReportNotFound_ShouldReturnNull()
        {
            // arrange
            var placeId = TestHelper.InsertTestDistrict("SomeTestDistrict1");
            var userId = TestHelper.InsertTestUser("jollytestuser", placeId);

            using (var service = UserReportAttributeServiceFactory.Instance.Create())
            {
                // act
                var result = service.GetUserReportStats(Guid.NewGuid(), userId, placeId);

                // assert
                Assert.Null(result);
            }
        }

        [Fact, AutoRollback]
        public void GetUserReportStats_ByReportEntityId()
        {
            // arrange
            var placeId = TestHelper.InsertTestDistrict("SomeTestDistrict1");
            var userId = TestHelper.InsertTestUser("jollytestuser", placeId);

            using (var service = UserReportAttributeServiceFactory.Instance.Create())
            {
                var reportEntityId = service.GetReportEntityId("Student\\Homeroom List with Counts", userId, placeId);

                service.TrackReportExecution(reportEntityId, userId, placeId);
                service.TrackReportExecution(reportEntityId, userId, placeId);

                // act
                var result = service.GetUserReportStats(reportEntityId, userId, placeId);

                // assert
                Assert.NotNull(result);
                Assert.Equal(DateTime.Today, result.LastRunDate.Value.Date);
                Assert.Equal(2, result.RunCount);
                Assert.False(result.IsFavorite);
            }
        }

        [Fact, AutoRollback]
        public void SetFavorite_ByReportEntityId()
        {
            // arrange
            var placeId = TestHelper.InsertTestDistrict("SomeTestDistrict1");
            var userId = TestHelper.InsertTestUser("jollytestuser", placeId);

            using (var service = UserReportAttributeServiceFactory.Instance.Create())
            {
                var reportEntityId = service.GetReportEntityId("Student\\Homeroom List with Counts", userId, placeId);

                // act
                service.SetFavorite(reportEntityId, userId, placeId);

                var result = service.GetUserReportStats(reportEntityId, userId, placeId);

                // assert
                Assert.NotNull(result);
                Assert.Null(result.LastRunDate);
                Assert.Equal(0, result.RunCount);
                Assert.True(result.IsFavorite);
            }
        }

        [Fact, AutoRollback]
        public void UnSetFavorite_ByReportEntityId()
        {
            // arrange
            var placeId = TestHelper.InsertTestDistrict("SomeTestDistrict1");
            var userId = TestHelper.InsertTestUser("jollytestuser", placeId);

            using (var service = UserReportAttributeServiceFactory.Instance.Create())
            {

                var reportEntityId = service.GetReportEntityId("Student\\Homeroom List with Counts", userId, placeId);

                // act
                service.SetFavorite(reportEntityId, userId, placeId);
                service.TrackReportExecution(reportEntityId, userId, placeId);
                service.UnsetFavorite(reportEntityId, userId, placeId);

                // assert
                var result = service.GetUserReportStats(reportEntityId, userId, placeId);
                Assert.NotNull(result);
                Assert.Equal(DateTime.Today,result.LastRunDate.Value.Date);
                Assert.Equal(1, result.RunCount);
                Assert.False(result.IsFavorite);
            }
        }

    }
#endif
}