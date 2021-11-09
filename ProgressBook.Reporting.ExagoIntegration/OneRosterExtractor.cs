using ProgressBook.LmsIntegration.OneRoster.Entities;
using ProgressBook.LmsIntegration.OneRoster.Repositories;
using ProgressBook.LmsIntegration.OneRoster.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using WebReports.Api.Common;
using DataColumn = System.Data.DataColumn;
using ServiceUtils = ProgressBook.LmsIntegration.OneRoster.Utilities.Utils;

namespace ProgressBook.Reporting.ExagoIntegration
{
    public class OneRosterExtractor
    {
        public static DataSet GetAcademicSessions(SessionInfo sessionInfo, int callType)
        {
            var masterConnectionString = sessionInfo.SetupData.DataSources.GetDataSource("PbMaster").DataConnStr;
            var sisConnectionString = sessionInfo.SetupData.DataSources.GetDataSource("StudentInformation").DataConnStr;
            var factory = new OneRosterServiceFactory(masterConnectionString);
            var irn = sessionInfo.SetupData.Parameters.GetParameter("DistrictIrn");
            var ds = new DataSet();
            DataTable sourceTable = new DataTable();
            sourceTable.Columns.AddRange(new[]
                {
                    new DataColumn("sourcedId", typeof(string)),
                    new DataColumn("title", typeof(string)),
                    new DataColumn("type", typeof(string)),
                    new DataColumn("startDate", typeof(string)),
                    new DataColumn("endDate", typeof(string)),
                    new DataColumn("schoolYear", typeof(string)),
                    new DataColumn("status", typeof(string)),
                    new DataColumn("dateLastModified", typeof(string))
                });
            if (!string.IsNullOrEmpty(irn?.Value) && callType > 0)
            {
                var academicSessionService = factory.GetAcademicSessionService(irn.Value);
                var sessions = academicSessionService.GetAll() as List<AcademicSessionModel>;
                foreach (AcademicSessionModel session in sessions)
                {
                    sourceTable.Rows.Add(
                        session.SourcedId,
                        session.Title,
                        ServiceUtils.GetDescriptionFromEnumValue(session.Type),
                        session.StartDate.ToIso8601Date(),
                        session.EndDate.ToIso8601Date(),
                        session.SchoolYear,
                        session.Status,
                        session.DateLastModified.ToIso8601DateTime()
                        );
                }
            }
            ds.Tables.Add(sourceTable);
            return ds;
        }

        public static DataSet GetOrgs(SessionInfo sessionInfo, int callType)
        {
            var masterConnectionString = sessionInfo.SetupData.DataSources.GetDataSource("PbMaster").DataConnStr;
            var sisConnectionString = sessionInfo.SetupData.DataSources.GetDataSource("StudentInformation").DataConnStr;
            var factory = new OneRosterServiceFactory(masterConnectionString);
            var irn = sessionInfo.SetupData.Parameters.GetParameter("DistrictIrn");
            var ds = new DataSet();
            DataTable sourceTable = new DataTable();
            sourceTable.Columns.AddRange(new[]
                {
                    new DataColumn("sourcedId", typeof(string)),
                    new DataColumn("name", typeof(string)),
                    new DataColumn("type", typeof(string)),
                    new DataColumn("parentSourcedId", typeof(string)),
                    new DataColumn("status", typeof(string)),
                    new DataColumn("dateLastModified", typeof(string))
                });
            if (!string.IsNullOrEmpty(irn?.Value) && callType > 0)
            {
                var orgService = factory.GetOrgService(irn.Value);
                var orgs = orgService.GetAll(irn.Value) as List<OrgModel>;
                foreach (var org in orgs)
                {
                    sourceTable.Rows.Add(
                        org.SourcedId,
                        org.Name,
                        org.Type.ToString().ToLower(),
                        org.Parent.SourcedId ?? "",
                        org.Status,
                        org.DateLastModified.ToIso8601DateTime()
                        );
                }
            }
            ds.Tables.Add(sourceTable);
            return ds;
        }

        public static DataSet GetCourses(SessionInfo sessionInfo, int callType)
        {
            var masterConnectionString = sessionInfo.SetupData.DataSources.GetDataSource("PbMaster").DataConnStr;
            var sisConnectionString = sessionInfo.SetupData.DataSources.GetDataSource("StudentInformation").DataConnStr;
            var factory = new OneRosterServiceFactory(masterConnectionString);
            var irn = sessionInfo.SetupData.Parameters.GetParameter("DistrictIrn");
            var ds = new DataSet();
            DataTable sourceTable = new DataTable();
            sourceTable.Columns.AddRange(new[]
                {
                    new DataColumn("sourcedId", typeof(string)),
                    new DataColumn("title", typeof(string)),
                    new DataColumn("courseCode", typeof(string)),
                    new DataColumn("grades", typeof(string)),
                    new DataColumn("orgSourcedId", typeof(string)),
                    new DataColumn("status", typeof(string)),
                    new DataColumn("dateLastModified", typeof(string))
                });

            if (!string.IsNullOrEmpty(irn?.Value) && callType > 0)
            {
                var courseService = factory.GetCourseService(irn.Value);
                var courses = courseService.GetAll() as List<CourseModel>;
                foreach (var course in courses)
                {
                    sourceTable.Rows.Add(
                        course.SourcedId,
                        course.Title,
                        course.CourseCode ?? "",
                        String.Join(",", course.Grades),
                        course.Org?.SourcedId ?? "",
                        course.Status,
                        course.DateLastModified.ToIso8601DateTime()
                    );
                }
            }
            ds.Tables.Add(sourceTable);
            return ds;
        }

        public static DataSet GetClasses(SessionInfo sessionInfo, int callType)
        {
            var masterConnectionString = sessionInfo.SetupData.DataSources.GetDataSource("PbMaster").DataConnStr;
            var sisConnectionString = sessionInfo.SetupData.DataSources.GetDataSource("StudentInformation").DataConnStr;
            var factory = new OneRosterServiceFactory(masterConnectionString);
            var irn = sessionInfo.SetupData.Parameters.GetParameter("DistrictIrn");
            var ds = new DataSet();
            DataTable sourceTable = new DataTable();
            sourceTable.Columns.AddRange(new[]
                {
                    new DataColumn("sourcedId", typeof(string)),
                    new DataColumn("title", typeof(string)),
                    new DataColumn("courseSourcedId", typeof(string)),
                    new DataColumn("classCode", typeof(string)),
                    new DataColumn("classType", typeof(string)),
                    new DataColumn("schoolSourcedId", typeof(string)),
                    new DataColumn("termSourcedIds", typeof(string)),
                    new DataColumn("status", typeof(string)),
                    new DataColumn("dateLastModified", typeof(string)),
                });

            if (!string.IsNullOrEmpty(irn?.Value) && callType > 0)
            {
                var classService = factory.GetClassService(irn.Value);
                var classes = classService.GetAll() as List<ClassModel>;
                foreach (var classRecord in classes)
                {
                    sourceTable.Rows.Add(
                        classRecord.SourcedId,
                        classRecord.Title,
                        classRecord.Course?.SourcedId ?? "",
                        classRecord.ClassCode ?? "",
                        ServiceUtils.GetDescriptionFromEnumValue(classRecord.ClassType),
                        classRecord.School?.SourcedId ?? "",
                        String.Join(",", classRecord.Terms.Select(i => i.SourcedId)),
                        classRecord.Status,
                        classRecord.DateLastModified.ToIso8601DateTime()
                    );
                }
            }
            ds.Tables.Add(sourceTable);
            return ds;
        }

        public static DataSet GetEnrollments(SessionInfo sessionInfo, int callType)
        {
            var masterConnectionString = sessionInfo.SetupData.DataSources.GetDataSource("PbMaster").DataConnStr;
            var sisConnectionString = sessionInfo.SetupData.DataSources.GetDataSource("StudentInformation").DataConnStr;
            var factory = new OneRosterServiceFactory(masterConnectionString);
            var irn = sessionInfo.SetupData.Parameters.GetParameter("DistrictIrn");
            var ds = new DataSet();
            DataTable sourceTable = new DataTable();
            sourceTable.Columns.AddRange(new[]
                {
                    new DataColumn("sourcedId", typeof(string)),
                    new DataColumn("classSourcedId", typeof(string)),
                    new DataColumn("schoolSourcedId", typeof(string)),
                    new DataColumn("userSourcedId", typeof(string)),
                    new DataColumn("role", typeof(string)),
                    new DataColumn("beginDate", typeof(string)),
                    new DataColumn("endDate", typeof(string)),
                    new DataColumn("status", typeof(string)),
                    new DataColumn("lastDateModified", typeof(string))
                });


            if (!string.IsNullOrEmpty(irn?.Value) && callType > 0)
            {
                var enrollmentService = factory.GetEnrollmentService(irn.Value);
                var enrollments = enrollmentService.GetAll().OfType<EnrollmentModel>().ToList();

                foreach (var enrollment in enrollments)
                {
                    sourceTable.Rows.Add(
                        enrollment.SourcedId,
                        enrollment.Class?.SourcedId ?? "",
                        enrollment.School?.SourcedId ?? "",
                        enrollment.User?.SourcedId ?? "",
                        ServiceUtils.GetDescriptionFromEnumValue(enrollment.Role),
                        enrollment.BeginDate == null ? string.Empty : enrollment.BeginDate.Value.ToIso8601Date(),
                        enrollment.EndDate == null ? string.Empty : enrollment.EndDate.Value.ToIso8601Date(),
                        enrollment.Status,
                        enrollment.DateLastModified.ToIso8601DateTime()
                    );
                }
            }
            ds.Tables.Add(sourceTable);
            return ds;
        }

        public static DataSet GetUsers(SessionInfo sessionInfo, int callType)
        {
            var masterConnectionString = sessionInfo.SetupData.DataSources.GetDataSource("PbMaster").DataConnStr;
            var sisConnectionString = sessionInfo.SetupData.DataSources.GetDataSource("StudentInformation").DataConnStr;
            var factory = new OneRosterServiceFactory(masterConnectionString);
            var irn = sessionInfo.SetupData.Parameters.GetParameter("DistrictIrn");
            var ds = new DataSet();
            DataTable sourceTable = new DataTable();
            sourceTable.Columns.AddRange(new[]
                {
                    new DataColumn("sourcedId", typeof(string)),
                    new DataColumn("enabledUser", typeof(string)),
                    new DataColumn("orgSourcedIds", typeof(string)),
                    new DataColumn("role", typeof(string)),
                    new DataColumn("username", typeof(string)),
                    new DataColumn("givenName", typeof(string)),
                    new DataColumn("familyName", typeof(string)),
                    new DataColumn("middleName", typeof(string)),
                    new DataColumn("identifier", typeof(string)),
                    new DataColumn("email", typeof(string)),
                    new DataColumn("status", typeof(string)),
                    new DataColumn("dateLastModified", typeof(string))
                });

            if (!string.IsNullOrEmpty(irn?.Value) && callType > 0)
            {
                var userService = factory.GetUserDemographicService(irn.Value, new DASLConnectionStringProvider(sisConnectionString));
                var users = userService.GetAll().OfType<UserModel>().ToList();
                foreach (var user in users)
                {
                    sourceTable.Rows.Add(
                        user.SourcedId,
                        user.EnabledUser.ToString().ToLower(),
                        String.Join(",", user.Orgs.Select(i => i.SourcedId)),
                        ServiceUtils.GetDescriptionFromEnumValue(user.Role),
                        user.Username,
                        user.GivenName,
                        user.FamilyName,
                        (user.MiddleName ?? ""),
                        (user.Identifier ?? ""),
                        (user.Email ?? ""),
                        user.Status,
                        user.DateLastModified.ToIso8601DateTime()
                    );
                }
            }
            ds.Tables.Add(sourceTable);
            return ds;
        }
    }

    public class DASLConnectionStringProvider : ISISConnectionStringProvider
    {
        private string _connectionString;
        public DASLConnectionStringProvider(string connectionString)
        {
            _connectionString = connectionString;
        }
        public string GetConnectionString()
        {
            return _connectionString;
        }
    }

    public static class DateTimeExtensions
    {
        public static string ToIso8601Date(this DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue)
            {
                return string.Empty;
            }
            return dateTime.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo);
        }

        public static string ToIso8601DateTime(this DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue)
            {
                return string.Empty;
            }
            return dateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);
        }
    }
}
