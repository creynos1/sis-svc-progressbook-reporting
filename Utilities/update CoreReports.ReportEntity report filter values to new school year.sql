
BEGIN TRAN


DECLARE @NewYear varchar(20)
	,	@OldYear varchar(20)

SET @NewYear = 
	(
		SELECT [Description] FROM tblSchoolYear 
		WHERE SchoolYear = '2019' and SchoolYearTypeId = 1
	)

SET @OldYear =
	(
		SELECT [Description] FROM tblSchoolYear 
		WHERE SchoolYear = '2018' and SchoolYearTypeId = 1
	)


UPDATE CoreReports.ReportEntity
SET		Content = REPLACE(Content, '<value>' + @OldYear + '</value>', '<value>' + @NewYear + '</value>')
	,	DateModified = getDate()

WHERE EntityType = 1 and Content like '%<value>' + @OldYear + '</value>%'


ROLLBACK TRAN
--COMMIT TRAN