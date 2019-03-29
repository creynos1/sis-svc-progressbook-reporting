<%@ Page Language="C#" EnableViewState="false" EnableEventValidation="false" %>
<%@ Register src="WebReportsCtrl.ascx" tagname="WebReportsCtrl" tagprefix="wr" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head id="Head1" runat="server">
        <title>Exago</title>
		<meta name="viewport" content="width=device-width, initial-scale=1">
        <style type="text/css">
            html, body { height:100%; width:100%; margin:0; padding:0; overflow:auto; background-color: #FEFDFD; }
			html { overflow: hidden; } /* Prevents "bounce scroll" on the body in MacOS and iOS */
            #WebReportsContainer { position:absolute; top:0; bottom:0; left:0; right:0; overflow:hidden; }
        </style>
    </head>
    <body>
        <form id="form1" runat="server">
            <div>
                <div id="WebReportsContainer">
                    <wr:WebReportsCtrl ID="WebReportsCtrl" runat="server" />
                </div>
            </div>
        </form>
        <style type="text/css">
        	/* Styles can be overidden here like this: .wrDynamicTabItemSelected { color:red; }
			Please duplicate this file (ExagoHome.aspx) to your own aspx page, since this file will be recreated during next install
            See technical guide for additional information */

			/* Sample style to add a small company logo to the top of the main left toolbar */
			/*.wrMainLeftPaneLogo {
				display: inline-block;
				background-image: url(Images/MyCompanyLogo.png);
			}*/
        </style>
    </body>
</html>
