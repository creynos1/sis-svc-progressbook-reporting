<%@ Page Language="C#" EnableViewState="false" EnableEventValidation="false" %>

<%@ Register Src="WebReportsCtrl.ascx" TagName="WebReportsCtrl" TagPrefix="wr" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Ad Hoc Reports</title>
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <style type="text/css">
        html, body {
            height: 100%;
            width: 100%;
            margin: 0;
            padding: 0;
            overflow: auto;
        }

        #WebReportsLogo {
            display: none;
        }

        #MainReportsContainer {
            top: 0px;
        }

        #WebReportsContainer {
            position: absolute;
            top: 0px;
            bottom: 0px;
            left: 0px;
            right: 0px;
            overflow: hidden;
            border: 1px solid #d5d5d5;
        }

        #WebReportsCtrl_MainSplitter_SplitterBarInner {
            top: 16px;
        }

        #WebReportsCtrl_MainSplitter_MainTabCtrl_HelpBtn {
            width: 0;
            height: 0;
            visibility: hidden;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <%--<img id="WebReportsLogo" alt="Logo" src="Images/head_eweb.png" />--%>
            <div id="WebReportsContainer">
                <wr:WebReportsCtrl ID="WebReportsCtrl" runat="server" />
            </div>
        </div>
    </form>
    <style type="text/css">
        /*
            .wrTreeSelectionMarker { background-color: #00467f; }
            .wrTreeItemSelected { color: #00467f; }
            .wrDynamicTabItemUnderline { background-color: #00467f; }
            */
    </style>
    <style type="text/css">
        	/* Styles can be overidden here like this: .wrDynamicTabItemSelected { color:red; }
			Please duplicate this file (ExagoHome.aspx) to your own aspx page, since this file will be recreated during next install
            See technical guide for additional information */
    </style>
    <script type="text/javascript">
        window.SaIntegration = {
            openHelp: function () {
                document.getElementById('WebReportsCtrl_MainSplitter_MainTabCtrl_HelpBtn').click();
            },
            enableUpload: function () {
                window.wrWebReportsCtrlClass.prototype.OnReportTreeRightClick = function (evt) {
                    var element = Utilities.GetEventSource(evt);
                    var x = evt.clientX;
                    var y = evt.clientY;
                    if (this.ReportFoldersCtrl.IsEmpty() && Utilities.Settings.AllowRootFolderActions) {
                        Utilities.PopupMenuCtrl.Show(this.emptyReportTreeContextMenuId, x, y, true);
                        return;
                    }
                    if (!Utilities.TreeCtrl.IsElementInNode(element)) {
                        return;
                    }
                    Utilities.TreeCtrl.OnClick(evt);
                    var node = Utilities.TreeCtrl.GetSelectedNode(this.reportsTreeId);
                    if (node == null) {
                        return;
                    }
                    var isReport = !this.ReportFoldersCtrl.IsFolder(node);
                    var isReadOnly = this.ReportFoldersCtrl.IsNodeReadOnly(node);
                    if (isReport) {
                        var showEditOptions = !isReadOnly;
                        Utilities.PopupMenuCtrl.ShowItem(this.reportContextMenuId, "edit", showEditOptions);
                        Utilities.PopupMenuCtrl.ShowItem(this.reportContextMenuId, "rename", showEditOptions);
                        Utilities.PopupMenuCtrl.ShowItem(this.reportContextMenuId, "delete", showEditOptions);
                        Utilities.PopupMenuCtrl.ShowItem(this.reportContextMenuId, "download", showEditOptions);
                    } else {
                        var showEditOptions = Utilities.Settings.AllowFolderManagement && !isReadOnly;
                        var showEditOptionsUpload = !isReadOnly;
                        Utilities.PopupMenuCtrl.ShowItem(this.folderContextMenuId, "addchild", showEditOptions);
                        Utilities.PopupMenuCtrl.ShowItem(this.folderContextMenuId, "rename", showEditOptions);
                        Utilities.PopupMenuCtrl.ShowItem(this.folderContextMenuId, "delete", showEditOptions);
                        Utilities.PopupMenuCtrl.ShowItem(this.folderContextMenuId, "newreport", showEditOptions);
                        var canMoveFolder = Utilities.Settings.MoveFolderMethodExists && showEditOptions;
                        Utilities.PopupMenuCtrl.ShowItem(this.folderContextMenuId, "movetoroot", canMoveFolder && Utilities.Settings.AllowRootFolderActions && this.ReportFoldersCtrl.GetParentFolder(this.ReportFoldersCtrl.GetFolderNodeFromElement(node)) != null);
                        Utilities.PopupMenuCtrl.ShowItem(this.folderContextMenuId, "upload", showEditOptionsUpload);
                    }
                    Utilities.PopupMenuCtrl.Show(isReport ? this.reportContextMenuId : this.folderContextMenuId, x, y, true);
                }
            }
        };
    </script>

    <% if (System.Configuration.ConfigurationManager.AppSettings["UploadEnabled"] == "true")
        { %>
    <script type="text/javascript">        
        window.SaIntegration.enableUpload();
    </script>
    <%} %>
</body>
</html>
