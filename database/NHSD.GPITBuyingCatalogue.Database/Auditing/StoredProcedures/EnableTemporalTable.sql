CREATE PROCEDURE [auditing].[EnableTemporalTable]
    @SchemaName nvarchar(100),
    @TableName nvarchar(100)
AS
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    BEGIN TRY
        DECLARE @TableExists bit = isnull((select object_id(@SchemaName + '.' + @TableName)), 0);
        DECLARE @SchemaId int = (Select schema_id from sys.schemas where name = @SchemaName)
        DECLARE @TableIsTemporal bit = isnull((select t.temporal_type from sys.tables t where t.name = @TableName and t.schema_id = @SchemaId and  t.temporal_type > 0), 0);
        DECLARE @sqlcmd NVARCHAR(MAX)

        IF @TableExists = 1 AND @TableIsTemporal = 0
        BEGIN		
            PRINT 'Enabling temporal table for ' + @SchemaName + '.' + @TableName
            SET @sqlcmd = 'ALTER TABLE ' + @SchemaName + '.' + @TableName + ' SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [' + @SchemaName + '].[' + @TableName + '_History]))'
            EXEC (@sqlcmd)
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
GO
