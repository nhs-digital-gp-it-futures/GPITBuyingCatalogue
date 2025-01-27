CREATE PROCEDURE [catalogue].[OptimizeIndexes]
AS
    DECLARE @indexes AS TABLE
      (
         [Schema]               SYSNAME NOT NULL,
         [Table]                SYSNAME NOT NULL,
         [Index]                SYSNAME NOT NULL,
         [FragmentationPercent] FLOAT NOT NULL
      )

    INSERT INTO @indexes
                ([Schema],
                 [Table],
                 [Index],
                 [FragmentationPercent])
    SELECT S.[name]                           AS 'Schema',
           T.[name]                           AS 'Table',
           I.[name]                           AS 'Index',
           DDIPS.avg_fragmentation_in_percent AS 'FragmentationPercent'
    FROM   .sys.dm_db_index_physical_stats (Db_id(), NULL, NULL, NULL, NULL) AS DDIPS
           INNER JOIN .sys.tables T
                   ON T.object_id = DDIPS.object_id
           INNER JOIN .sys.schemas S
                   ON T.schema_id = S.schema_id
           INNER JOIN .sys.indexes I
                   ON I.object_id = DDIPS.object_id
                      AND DDIPS.index_id = I.index_id
    WHERE  DDIPS.database_id = Db_id()
           AND I.[name] IS NOT NULL
           AND DDIPS.avg_fragmentation_in_percent > 5
    ORDER  BY DDIPS.avg_fragmentation_in_percent DESC

    DECLARE @sqlcmd VARCHAR(max) = (SELECT STRING_AGG(CONCAT('ALTER INDEX ALL ON [',[Schema], '].[', [Table], '] ', (CASE WHEN FragmentationPercent > 40 THEN 'REBUILD' ELSE 'REORGANIZE' END)), ';' + Char(10)) FROM @indexes)

    EXEC(@sqlcmd)
RETURN 0
