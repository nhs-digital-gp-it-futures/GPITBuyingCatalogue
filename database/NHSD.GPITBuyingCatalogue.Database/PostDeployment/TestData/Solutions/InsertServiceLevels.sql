IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE' AND NOT EXISTS (SELECT * FROM [catalogue].[ServiceAvailabilityTimes])
BEGIN
    INSERT [catalogue].[ServiceLevelAgreements] ([SolutionId], [SlaType]) VALUES (N'10000-001', 1)
    INSERT [catalogue].[ServiceLevelAgreements] ([SolutionId], [SlaType]) VALUES (N'10000-002', 1)
    INSERT [catalogue].[ServiceLevelAgreements] ([SolutionId], [SlaType]) VALUES (N'10029-003', 1)
    INSERT [catalogue].[ServiceLevelAgreements] ([SolutionId], [SlaType]) VALUES (N'10030-001', 1)
    INSERT [catalogue].[ServiceLevelAgreements] ([SolutionId], [SlaType]) VALUES (N'99999-89', 1)
    
    SET IDENTITY_INSERT [catalogue].[ServiceAvailabilityTimes] ON
    INSERT [catalogue].[ServiceAvailabilityTimes] ([Id], [SolutionId], [Category], [TimeFrom], [TimeUntil], [ApplicableDays]) VALUES (11, N'10030-001', N'Core support hours', CAST(N'2021-11-12T06:30:00.0000000' AS DateTime2), CAST(N'2021-11-12T20:30:00.0000000' AS DateTime2), N'Monday – Sunday inclusive and including Bank Holidays.')
    INSERT [catalogue].[ServiceAvailabilityTimes] ([Id], [SolutionId], [Category], [TimeFrom], [TimeUntil], [ApplicableDays]) VALUES (12, N'10030-001', N'Non-core support hours', CAST(N'2021-11-12T20:30:00.0000000' AS DateTime2), CAST(N'2021-11-12T06:30:00.0000000' AS DateTime2), N'Monday – Sunday inclusive and including Bank Holidays.')
    INSERT [catalogue].[ServiceAvailabilityTimes] ([Id], [SolutionId], [Category], [TimeFrom], [TimeUntil], [ApplicableDays]) VALUES (13, N'10000-002', N'Core support hours', CAST(N'2021-11-12T06:30:00.0000000' AS DateTime2), CAST(N'2021-11-12T20:30:00.0000000' AS DateTime2), N'Monday – Sunday inclusive and including Bank Holidays.')
    INSERT [catalogue].[ServiceAvailabilityTimes] ([Id], [SolutionId], [Category], [TimeFrom], [TimeUntil], [ApplicableDays]) VALUES (14, N'10000-002', N'Non-core support hours', CAST(N'2021-11-12T20:30:00.0000000' AS DateTime2), CAST(N'2021-11-12T06:30:00.0000000' AS DateTime2), N'Monday – Sunday inclusive and including Bank Holidays.')
    INSERT [catalogue].[ServiceAvailabilityTimes] ([Id], [SolutionId], [Category], [TimeFrom], [TimeUntil], [ApplicableDays]) VALUES (15, N'10000-001', N'Core support hours', CAST(N'2021-11-12T06:30:00.0000000' AS DateTime2), CAST(N'2021-11-12T20:30:00.0000000' AS DateTime2), N'Monday – Sunday inclusive and including Bank Holidays.')
    INSERT [catalogue].[ServiceAvailabilityTimes] ([Id], [SolutionId], [Category], [TimeFrom], [TimeUntil], [ApplicableDays]) VALUES (16, N'10000-001', N'Non-core support hours', CAST(N'2021-11-12T20:30:00.0000000' AS DateTime2), CAST(N'2021-11-12T06:30:00.0000000' AS DateTime2), N'Monday – Sunday inclusive and including Bank Holidays.')
    INSERT [catalogue].[ServiceAvailabilityTimes] ([Id], [SolutionId], [Category], [TimeFrom], [TimeUntil], [ApplicableDays]) VALUES (17, N'99999-89', N'Core support hours', CAST(N'2021-11-12T06:30:00.0000000' AS DateTime2), CAST(N'2021-11-12T20:30:00.0000000' AS DateTime2), N'Monday – Sunday inclusive and including Bank Holidays.')
    INSERT [catalogue].[ServiceAvailabilityTimes] ([Id], [SolutionId], [Category], [TimeFrom], [TimeUntil], [ApplicableDays]) VALUES (18, N'99999-89', N'Non-core support hours', CAST(N'2021-11-12T20:30:00.0000000' AS DateTime2), CAST(N'2021-11-12T06:30:00.0000000' AS DateTime2), N'Monday – Sunday inclusive and including Bank Holidays.')
    INSERT [catalogue].[ServiceAvailabilityTimes] ([Id], [SolutionId], [Category], [TimeFrom], [TimeUntil], [ApplicableDays]) VALUES (19, N'10029-003', N'Core support hours', CAST(N'2021-11-12T06:30:00.0000000' AS DateTime2), CAST(N'2021-11-12T20:30:00.0000000' AS DateTime2), N'Monday – Sunday inclusive and including Bank Holidays.')
    INSERT [catalogue].[ServiceAvailabilityTimes] ([Id], [SolutionId], [Category], [TimeFrom], [TimeUntil], [ApplicableDays]) VALUES (20, N'10029-003', N'Non-core support hours', CAST(N'2021-11-12T20:30:00.0000000' AS DateTime2), CAST(N'2021-11-12T06:30:00.0000000' AS DateTime2), N'Monday – Sunday inclusive and including Bank Holidays.')
    SET IDENTITY_INSERT [catalogue].[ServiceAvailabilityTimes] OFF
END
GO
