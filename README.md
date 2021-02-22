# sis-svc-progressbook-reporting
Exago integration components for the ProgressBook Suite

# Setup
### Prerequisites
1. [Master Database](https://github.com/FrontlineEducation/sis-db-gradebook-master)
2. 1 or more [District Database](https://github.com/FrontlineEducation/sis-db-gradebook-district)
3. [SIS Database](https://github.com/FrontlineEducation/sis-db-studentinformation)
3. [SpecialServices](https://github.com/FrontlineEducation/sis-app-specialservices) (database only)
4. pb_template (document pending)
5. IIS (document pending)
6. [Central](https://github.com/FrontlineEducation/sis-app-centraladmin)
7. [Student Information](httos://github.com/FrontlineEducation/sis-app-studentinformation)

### Build the Solution
Clone the sis-svc-progressbook-reporting repository

Open the ProgressBook.Reporting solution in visual studio

If not already done, setup the nuget package source in visual studio
1.	Tools -> NuGet Package Manager -> Manage NuGet Packages for Solution…
2.	Gear icon next to package source dropdown at the top right
3.	Create a new source and name it anything
4.	Source: `https://tfs1.software-answers.com/ProgressBookCollection/_packaging/SA_Nuget/nuget/v3/index.json`
5.	Restore NuGet packages for the solution

There are config files in several projects that need to be manually copied and renamed. For example, `ProgressBook.Reporting.Web\Config\connectionStrings.config.sample` should be copied and renamed to `ProgressBook.Reporting.Web\Config\connectionStrings.config` (keep the sample file). You can use this powershell script to do it automatically
```powershell
$sampleFiles = Get-ChildItem -filter *.sample -Recurse | Select-Object FullName
$sampleFiles | ForEach-Object{   
   Copy-Item $_.FullName -Destination ($_.FullName -replace '.sample', '')
} 
```
1.	ProgressBook.Reporting.Web\Config

You should now be able to build the solution. You may need to build once in Release mode.

### Configuration

With the ProgressBook.Reporting solution open in visual studio, go to the properties of the ProgressBook\Web.ProgressBook project

Navigate to the Web tab, and in the Servers section at the bottom...
1.	Change the dropdown value to Local IIS
2.	Change the Project Url to https://(your local website)/Reporting
3.	Click Create Virtual Directory, then save

Update the ProgressBook connection string with your central database
1.	Data Source will be your SQL server name
2.	Initial Catalog will be your Central database name
3.	User Id and Password will be for your SQL user

Update the StudentInformation connection string the same way, but with the sis database instead.

Update the DistrictTemplate connection string the same way, but with the pb_template database instead.

Update the PbMasterContext connection string the same way, but with the pb_master database instead.

Update the PbMasterContext connection string the same way, but with the pb_master database instead.

Update the QuickReports connection string the same way, but with the sis database instead (use the same values as StudentInformation).

Update the SpecialServices connection string the same way, but with the special services database instead.

You will need to update these in the following locations:
1.	ProgressBook\config\connectionStrings.config

Rebuild the ProgressBook.Reporting.Web solution to make sure everything is still working.

Register the Reporting service in Central
1.	Log in to Central in your browser
2.	In the dropdown in the top right, click CentralAdmin Management
3.	Click Settings on the left
4.	In the field labeled **Reporting URL** enter the URL you created earlier (https://(your local)/Reporting/)
6.	Click **Save**

Navigate to https://(your local)/Reporting/. You should receive a 403 Forbidden error. If you receive an ASP.Net error page, you must resolve that error before proceeding.

Configure the Exago website
1. Open Internet Information Services (IIS) Manager)
2. Navigate to Sites -> Default Web Site -> Reporting -> Exago
3. Right click on the Exago folder and click **Convert to Application**
4. Click **OK**
5. Grant **IIS_IUSRS** modify access to the `Exago\Config` folder
    1. Within the Exago folder, right click the Config folder and select **Edit Permissions**
    2. On the **Security** tab, click **Edit**
    3. Click **Add**
        8. In the prompt **Enter the object names to select** type `IIS_IUSRS` then click **Check Names**
    4. Click **OK**
    5. In the section labelled **Permissions for IIS_IUSRS**, check **Modify**
    6. Click **OK
6. Repeat the steps above to grant **IIS_IUSRS** modify access to the `Exago\Temp` folder

### Install the Exago Scheduler

Create a local service account
1. Open the Windows start menu.
2. Search for "Edit local users and groups".
3. Right click the **Users** folder and click **New User**.
4. Enter a user name and password.
5. Uncheck **User must change password at next logon**.
6. Check **User cannot change password** and **Password never expires**.
7. Click **Create**.

Add the service account to the **Performance Monitor Users** group
1. Open the Windows start menu.
2. Search for "Edit local users and groups".
3. Select the **Groups** folder.
4. Double click **Performance Monitor Users**.
5. Click **Add**.
6. In the prompt **Enter the object names to select** type your service account then click **Check Names**.
7. Click **OK**.
8. Click **OK** again.

Navigate to the folder `ProgressBook.Reporting.ExagoScheduler.Setup\bin\Release`

Run `ProgressBook.Reporting.ExagoScheduler.Setup.exe`

Follow the steps of the installation wizard, leaving all settings with their default vaules.

Once the installation is complete, the **Exago Scheduler Settings** tool will launch.
1. On the **Email** tab, enter `localhost` for **SMTP Server** and your email address for **From Email**. You may leave the remaining fields with their default values.
2. On the **Database** tab, enter connection information for all databases listed.
3. Skip the **Service** tab.
4. On the **Advanced** tab, enter the `JAMSReport` value from your SIS fileStore.config for **Report Path** and your email adderss for **Error Report Email**. You may leave the remaining fields with their default values.
5. Skip the **FTP Logging** tab.
6. Close the window to save your changes.

Grant your service account modify access to:
1. `C:\Program Files\Exago\ExagoScheduler`
2. The **Report Path** configured above
3. `C:\ProgramData\Software Answers\Ad Hoc Reports\Integration`

Configure the Exago Schedule to use your service account.
1. In Visual Studio, change your build configuration to **Debug** 
2. Change the startup project to **ProgressBook.Reporting.ExagoScheduler.Config**
3. Run the project.
4. On the Service tab, enter your service account username and password. Be sure to include your machine name as the domain in the username.
5. Close the window to save your changes.

Confirm your service is running
1. Open the Windows start menu.
2. Search for "Services".
3. Look for "ExagoScheduler" in the list of services.
4. Verify **Status** is "Running".
5. Verify **Lon On As** is set to your service account.

### Grant your ProgressBook account access to reports

1. Sign in to your StudentInformation applicaiton.
2. Navigate to the **View Accounts** page
3. Search for your SIS account.
4. Edit your account.
5. Ensure your user is a member of the **AH-Full Admin Report Manager** for school **All Buildings**.
6. Sign out and back in to SIS to recalculate your security.	
7. After signing back in, navivate to the **Report Designer** and verify everything loads correctly.
