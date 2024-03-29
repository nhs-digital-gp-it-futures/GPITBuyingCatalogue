﻿IF (UPPER('$(INSERT_TEST_DATA)') = 'TRUE')
BEGIN
    /*********************************************************************************************************************************************/
    /* FrameworkSolutions */
    /*********************************************************************************************************************************************/

    CREATE TABLE #FrameworkSolutions
    (
        FrameworkId nvarchar(36) NOT NULL,
        SolutionId nvarchar(14) NOT NULL,
        IsFoundation bit CONSTRAINT DF_FrameworkSolutions_IsFoundation DEFAULT 0 NOT NULL,
        LastUpdated datetime2(7) NOT NULL,
        LastUpdatedBy int NULL,
    );

    DECLARE @bobEmail AS nvarchar(50) = N'BobSmith@email.com';
    DECLARE @bobUser AS int = (SELECT Id FROM users.AspNetUsers WHERE Email = @bobEmail);

    INSERT INTO #FrameworkSolutions (FrameworkId, SolutionId, IsFoundation, LastUpdated, LastUpdatedBy)
         VALUES (N'TIF001', N'10000-001', 1, CAST(N'2020-03-25T07:30:18.1133333' AS datetime2), @bobUser),
                (N'TIF001', N'10000-002', 0, CAST(N'2020-04-06T10:50:03.2166667' AS datetime2), @bobUser),
                (N'DFOCVC001', N'10000-054', 0, CAST(N'2020-04-03T12:25:59.0566667' AS datetime2), @bobUser),
                (N'TIF001', N'10000-062', 0, CAST(N'2020-04-06T10:53:50.6266667' AS datetime2), @bobUser),
                (N'DFOCVC001', N'10000-062', 0, CAST(N'2020-04-06T10:53:50.6266667' AS datetime2), @bobUser),
                (N'TIF001', N'10004-001', 0, CAST(N'2020-03-26T12:13:20.0866667' AS datetime2), @bobUser),
                (N'TIF001', N'10004-002', 0, CAST(N'2020-03-30T13:14:43.1666667' AS datetime2), @bobUser),
                (N'TIF001', N'10007-002', 0, CAST(N'2020-03-25T11:40:44.2900000' AS datetime2), @bobUser),
                (N'TIF001', N'10011-003', 0, CAST(N'2020-06-18T14:20:53.8233333' AS datetime2), @bobUser),
                (N'TIF001', N'10020-001', 0, CAST(N'2020-04-06T12:50:27.8800000' AS datetime2), @bobUser),
                (N'TIF001', N'10029-001', 0, CAST(N'2020-04-08T07:42:58.2633333' AS datetime2), @bobUser),
                (N'TIF001', N'10029-003', 0, CAST(N'2020-04-08T08:59:03.8100000' AS datetime2), @bobUser),
                (N'TIF001', N'10030-001', 0, CAST(N'2020-04-01T10:39:24.7100000' AS datetime2), @bobUser),
                (N'TIF001', N'10031-001', 0, CAST(N'2020-04-01T10:37:59.3066667' AS datetime2), @bobUser),
                (N'TIF001', N'10033-001', 0, CAST(N'2020-04-01T10:40:33.7566667' AS datetime2), @bobUser),
                (N'TIF001', N'10035-001', 0, CAST(N'2020-04-01T10:42:08.5066667' AS datetime2), @bobUser),
                (N'TIF001', N'10046-001', 0, CAST(N'2020-03-30T13:02:24.5200000' AS datetime2), @bobUser),
                (N'TIF001', N'10046-003', 0, CAST(N'2020-03-30T13:04:21.6500000' AS datetime2), @bobUser),
                (N'ATP001', N'10046-006', 0, CAST(N'2020-06-25T14:31:07.2366667' AS datetime2), @bobUser),
                (N'ATP001', N'10047-001', 0, CAST(N'2020-04-01T10:43:15.8533333' AS datetime2), @bobUser),
                (N'ATP001', N'10052-002', 1, CAST(N'2020-03-30T13:19:48.8766667' AS datetime2), @bobUser),
                (N'ATP001', N'10059-001', 0, CAST(N'2020-03-30T13:16:49.4100000' AS datetime2), @bobUser),
                (N'ATP001', N'10062-001', 0, CAST(N'2020-04-03T12:28:52.3800000' AS datetime2), @bobUser),
                (N'ATP001', N'10063-002', 0, CAST(N'2020-06-25T14:30:56.3300000' AS datetime2), @bobUser),
                (N'ATP001', N'10064-003', 0, CAST(N'2020-06-25T14:30:49.8600000' AS datetime2), @bobUser),
                (N'ATP001', N'10072-003', 0, CAST(N'2020-06-25T14:30:33.5166667' AS datetime2), @bobUser),
                (N'ATP001', N'10072-004', 0, CAST(N'2020-06-25T14:31:34.0466667' AS datetime2), @bobUser),
                (N'NHSDGP001', N'10072-006', 0, CAST(N'2020-06-25T14:31:15.0200000' AS datetime2), @bobUser),
                (N'NHSDGP001', N'10073-009', 0, CAST(N'2020-04-01T12:49:33.9433333' AS datetime2), @bobUser);

    MERGE INTO catalogue.FrameworkSolutions AS TARGET
    USING #FrameworkSolutions AS SOURCE
    ON TARGET.FrameworkId = SOURCE.FrameworkId AND TARGET.SolutionId = SOURCE.SolutionId
    WHEN MATCHED THEN
           UPDATE SET TARGET.IsFoundation = SOURCE.IsFoundation,
                      TARGET.LastUpdated = SOURCE.LastUpdated,
                      TARGET.LastUpdatedBy = SOURCE.LastUpdatedBy
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (FrameworkId, SolutionId, IsFoundation, LastUpdated, LastUpdatedBy)
        VALUES (SOURCE.FrameworkId, SOURCE.SolutionId, SOURCE.IsFoundation, SOURCE.LastUpdated, SOURCE.LastUpdatedBy);
END;
GO
