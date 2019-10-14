using System.Collections.Generic;
using System.Data;
using ExagoReportUtility.Models;
namespace ExagoReportUtility.Objects
{
    class ExagoDataList
    {
        public IList<ReportFilter> exagoFilterList;
        public IList<ReportSort> exagoSortList;
        public IList<ReportEntity> reportEntityList;

        public ExagoDataList()
        {
            exagoFilterList = new List<ReportFilter>();
            exagoSortList = new List<ReportSort>();
            reportEntityList = new List<ReportEntity>();
        }



        public void MapFriendlySort(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                var friendlySort = new ReportSort();
                friendlySort.ExagoSortName = row["ExagoSortName"].ToString();
                friendlySort.SoftwareAnswersSortName = row["FriendlySortName"].ToString();
                exagoSortList.Add(friendlySort);
            }

        }

        public void MapFriendlyFilter(DataTable dataTable)
        {
            foreach(DataRow row in dataTable.Rows)
            {
                var friendlyFilter = new ReportFilter();
                friendlyFilter.ExagoFilterName = row["ExagoFilterName"].ToString();
                friendlyFilter.SoftwareAnswersFilterName = row["FriendlyFilterName"].ToString();
                exagoFilterList.Add(friendlyFilter);
            }
        }

        public void MapReportEntities(DataTable dataTable)
        {

            foreach (DataRow row in dataTable.Rows)
            {
                var reportId = row["ReportEntityId"].ToString();
                var parentId = row["ParentId"].ToString();
                var content = row["Content"].ToString();
                var reportEntity = new ReportEntity(reportId, parentId, content);
                reportEntityList.Add(reportEntity);

            }
        }


    }
}
