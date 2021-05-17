IF (UPPER('$(INSERT_TEST_DATA)') = 'TRUE')
BEGIN
    /*********************************************************************************************************************************************/
    /* MarketingContact */
    /*********************************************************************************************************************************************/

    CREATE TABLE #MarketingContact
    (
        Id int NOT NULL,
        SolutionId nvarchar(14) NOT NULL,
        FirstName nvarchar(35) NULL,
        LastName nvarchar(35) NULL,
        Email nvarchar(255) NULL,
        PhoneNumber nvarchar(35) NULL,
        Department nvarchar(50) NULL,
        LastUpdated datetime2(7) NOT NULL,
        LastUpdatedBy uniqueidentifier NOT NULL,
    );

    INSERT INTO #MarketingContact (Id, SolutionId, FirstName, LastName, Email, PhoneNumber, Department, LastUpdated, LastUpdatedBy) 
         VALUES (1003, N'10000-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-03-31T10:50:17.6233333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1049, N'10000-002', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-08T10:55:58.5100000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1046, N'10000-054', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-07T19:57:45.7166667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1045, N'10000-062', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-07T19:47:00.3600000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1002, N'10004-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-03-26T13:24:14.8666667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1009, N'10004-002', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-03-31T11:46:17.2700000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1001, N'10007-002', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-03-25T12:12:59.8166667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1043, N'10020-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-07T19:21:57.3566667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1044, N'10020-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-07T19:21:57.3566667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1047, N'10029-003', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-08T10:05:14.8833333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1048, N'10029-003', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-08T10:05:14.8866667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1036, N'10030-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-02T16:28:19.8700000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1037, N'10030-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-02T16:28:19.8700000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1017, N'10031-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-01T12:50:18.1366667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1018, N'10031-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-01T12:50:18.1366667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1029, N'10033-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-01T14:09:25.0333333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1013, N'10035-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-01T12:22:15.4700000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1014, N'10035-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-01T12:22:15.4700000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1012, N'10046-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-03-31T14:40:22.0600000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1010, N'10046-003', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-03-31T14:07:29.2166667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1011, N'10046-003', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-03-31T14:07:29.2200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1030, N'10047-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-01T15:11:13.1100000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1031, N'10047-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-01T15:11:13.1133333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1034, N'10052-002', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-02T12:24:23.3200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1035, N'10052-002', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-02T12:24:23.3200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1006, N'10059-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-03-31T11:03:30.5666667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1007, N'10059-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-03-31T11:03:30.5666667' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1038, N'10062-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-07T09:45:13.5733333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1039, N'10062-001', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-07T09:45:13.5733333' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1027, N'10073-009', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-01T13:56:52.4200000' AS datetime2), N'00000000-0000-0000-0000-000000000000'),
                (1028, N'10073-009', N'Bob', N'Smith', N'bob.smith@anon.net', N'01234 5678901', N'Internal Sales Team', CAST(N'2020-04-01T13:56:52.4200000' AS datetime2), N'00000000-0000-0000-0000-000000000000');

    SET IDENTITY_INSERT dbo.MarketingContact ON; 

    MERGE INTO dbo.MarketingContact AS TARGET
    USING #MarketingContact AS SOURCE
    ON TARGET.Id = SOURCE.Id 
    WHEN MATCHED THEN  
           UPDATE SET TARGET.SolutionId = SOURCE.SolutionId,
                      TARGET.FirstName = SOURCE.FirstName,
                      TARGET.LastName = SOURCE.LastName,
                      TARGET.Email = SOURCE.Email,
                      TARGET.PhoneNumber = SOURCE.PhoneNumber,
                      TARGET.Department = SOURCE.Department,
                      TARGET.LastUpdated = SOURCE.LastUpdated,
                      TARGET.LastUpdatedBy = SOURCE.LastUpdatedBy
    WHEN NOT MATCHED BY TARGET THEN  
        INSERT (Id, SolutionId, FirstName, LastName, Email, PhoneNumber, Department, LastUpdated, LastUpdatedBy) 
        VALUES (SOURCE.Id, SOURCE.SolutionId, SOURCE.FirstName, SOURCE.LastName, SOURCE.Email, SOURCE.PhoneNumber, SOURCE.Department, SOURCE.LastUpdated, SOURCE.LastUpdatedBy);

    SET IDENTITY_INSERT dbo.MarketingContact OFF; 
END;
GO
