<%@ Page Language="C#" EnableViewState="false" EnableEventValidation="false" %>
<%@ Register src="WebReportsCtrl.ascx" tagname="WebReportsCtrl" tagprefix="wr" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head id="Head1" runat="server">
        <title>Exago</title>
		<meta name="viewport" content="width=device-width, initial-scale=1">
        <style type="text/css">
            #WebReportsContainer { position:absolute; top:0; bottom:0; left:0; right:0; overflow:hidden; }
        </style>
		<%-- Uncomment/update the following line to set the domain on this page to the host application's domain --%>
		<%-- <script>(function () { document.domain = 'mydomain.com'; })()</script> --%>
    </head>
    <body>
        <form id="form1" runat="server">
            <div>
                <div id="WebReportsContainer">
                    <wr:WebReportsCtrl ID="WebReportsCtrl" runat="server" />
                </div>
            </div>
        </form>
    </body>
</html>
