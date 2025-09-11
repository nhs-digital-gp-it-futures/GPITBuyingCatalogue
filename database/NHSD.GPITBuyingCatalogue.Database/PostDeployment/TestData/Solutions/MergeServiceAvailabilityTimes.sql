IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE'
BEGIN
    SET IDENTITY_INSERT [catalogue].[ServiceAvailabilityTimes] ON

    MERGE INTO [catalogue].[ServiceAvailabilityTimes] AS TARGET
    USING (
    VALUES
        (23, N'10030-001', N'Core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (24, N'10030-001', N'Non-core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (25, N'10059-001', N'Core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (26, N'10059-001', N'Non-core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (27, N'10033-001', N'Core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (28, N'10033-001', N'Non-core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (29, N'10000-002', N'Core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (30, N'10000-002', N'Non-core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (31, N'100001-001', N'Core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (32, N'100001-001', N'Non-core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (33, N'10047-001', N'Core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (34, N'10047-001', N'Non-core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (35, N'10004-001', N'Core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (36, N'10004-001', N'Non-core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (37, N'10007-002', N'Core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (38, N'10007-002', N'Non-core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (39, N'100007-002', N'Core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (40, N'100007-002', N'Non-core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (41, N'99999-01', N'Core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (42, N'99999-01', N'Non-core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (43, N'100004-001', N'Core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (44, N'100004-001', N'Non-core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (45, N'10046-001', N'Core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (46, N'10046-001', N'Non-core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (47, N'10046-003', N'Core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (48, N'10046-003', N'Non-core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (49, N'100005-001', N'Core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (50, N'100005-001', N'Non-core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (51, N'10000-001', N'Core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (52, N'10000-001', N'Non-core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (53, N'10035-001', N'Core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (54, N'10035-001', N'Non-core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (55, N'10004-002', N'Core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (56, N'10004-002', N'Non-core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (57, N'100003-001', N'Core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (58, N'100003-001', N'Non-core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (59, N'100007-001', N'Core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (60, N'100007-001', N'Non-core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (61, N'99999-89', N'Core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (62, N'99999-89', N'Non-core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (63, N'99998-98', N'Core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (64, N'99998-98', N'Non-core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (65, N'10073-009', N'Core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (66, N'10073-009', N'Non-core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (67, N'10029-003', N'Core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (68, N'10029-003', N'Non-core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (69, N'10052-002', N'Core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (70, N'10052-002', N'Non-core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (71, N'10000-062', N'Core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (72, N'10000-062', N'Non-core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (73, N'100000-001', N'Core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (74, N'100000-001', N'Non-core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (75, N'100002-001', N'Core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2),
        (76, N'100002-001', N'Non-core support hours', GETUTCDATE(), GETUTCDATE(), NULL, N'0,1,2,3,4,5,6', 1, NULL, GETUTCDATE(), 2))
    AS SOURCE ([Id], [SolutionId], [Category], [TimeFrom], [TimeUntil], [ApplicableDays], [IncludedDays], [IncludesBankHolidays], [AdditionalInformation], [LastUpdated], [LastUpdatedBy])
    ON TARGET.[Id] = SOURCE.[Id]
    WHEN MATCHED THEN UPDATE SET
        TARGET.[SolutionId] = SOURCE.[SolutionId],
        TARGET.[Category] = SOURCE.[Category],
        TARGET.[TimeFrom] = SOURCE.[TimeFrom],
        TARGET.[TimeUntil] = SOURCE.[TimeUntil],
        TARGET.[ApplicableDays] = SOURCE.[ApplicableDays],
        TARGET.[IncludedDays] = SOURCE.[IncludedDays],
        TARGET.[IncludesBankHolidays] = SOURCE.[IncludesBankHolidays],
        TARGET.[AdditionalInformation] = SOURCE.[AdditionalInformation],
        TARGET.[LastUpdated] = SOURCE.[LastUpdated],
        TARGET.[LastUpdatedBy] = SOURCE.[LastUpdatedBy]
    WHEN NOT MATCHED BY TARGET THEN
        INSERT ([Id], [SolutionId], [Category], [TimeFrom], [TimeUntil], [ApplicableDays], [IncludedDays], [IncludesBankHolidays], [AdditionalInformation], [LastUpdated], [LastUpdatedBy])
        VALUES (SOURCE.[Id], SOURCE.[SolutionId], SOURCE.[Category], SOURCE.[TimeFrom], SOURCE.[TimeUntil], SOURCE.[ApplicableDays], SOURCE.[IncludedDays], SOURCE.[IncludesBankHolidays], SOURCE.[AdditionalInformation], SOURCE.[LastUpdated], SOURCE.[LastUpdatedBy]);

    SET IDENTITY_INSERT [catalogue].[ServiceAvailabilityTimes] OFF
END

GO
