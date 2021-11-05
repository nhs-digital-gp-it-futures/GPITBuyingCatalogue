CREATE PROCEDURE [auditing].[DisableTemporalTable]
     @SchemaName nvarchar(100),
	 @TableName nvarchar(100)
AS
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    BEGIN TRY
		DECLARE @TableExists bit = isnull((select object_id(@SchemaName + '.' + @TableName)), 0);
		DECLARE @SchemaId int = (Select schema_id from sys.schemas where name = @SchemaName)
		DECLARE @TableIsTemporal bit = isnull((select t.temporal_type from sys.tables t where t.name = @TableName and t.schema_id = @SchemaId and  t.temporal_type > 0), 0);

		DECLARE @sqlcmd VARCHAR(MAX)

		IF @TableExists = 1 AND @TableIsTemporal = 1
		BEGIN
            PRINT 'Disabling temporal table for ' + @SchemaName + '.' + @TableName
			SET @sqlcmd = 'ALTER TABLE ' + @SchemaName + '.' + @TableName + ' SET (SYSTEM_VERSIONING = OFF)'
			EXEC (@sqlcmd)
			SET @sqlcmd = 'ALTER TABLE ' + @SchemaName + '.' + @TableName + ' DROP PERIOD FOR SYSTEM_TIME'
			EXEC (@sqlcmd)
			SET @sqlcmd = 'ALTER TABLE ' + @SchemaName + '.' + @TableName + ' DROP DF_' + @SchemaName + '_' + @TableName + '_SysStart, DF_' + @SchemaName + '_' + @TableName + '_SysEnd'
			EXEC (@sqlcmd)
			SET @sqlcmd = 'ALTER TABLE ' + @SchemaName + '.' + @TableName + ' DROP COLUMN SysStartTime, COLUMN SysEndTime'
			EXEC (@sqlcmd)
			SET @sqlcmd = 'DROP TABLE [' + @SchemaName + '].[' + @TableName + '_History]'
			EXEC (@sqlcmd)
		END
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
GO
