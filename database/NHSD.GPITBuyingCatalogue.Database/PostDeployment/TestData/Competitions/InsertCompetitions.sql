IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE' AND NOT EXISTS (SELECT * FROM competitions.Competitions)
BEGIN

    DECLARE @organisationId NVARCHAR(3) = (SELECT Id FROM organisations.Organisations WHERE ExternalIdentifier = 'QWO');

    SET IDENTITY_INSERT [competitions].[Competitions] ON 

    INSERT [competitions].[Competitions] ([Id], [Name], [Description], [FilterId], [OrganisationId], [LastUpdated], [ShortlistAccepted], [IsDeleted]) VALUES (1, N'Competition at Solution Selection stage', N'This competition will take you directly to the solution selection stage as no solutions have yet been shortlisted', 2, @organisationId, GETUTCDATE(), NULL, 0)
    INSERT [competitions].[Competitions] ([Id], [Name], [Description], [FilterId], [OrganisationId], [LastUpdated], [ShortlistAccepted], [IsDeleted]) VALUES (2, N'Competition at Solution Justification stage', N'This competition will take you to the stage of justifying why you haven''t picked certain solutions.', 2, @organisationId, GETUTCDATE(), NULL, 0)
    INSERT [competitions].[Competitions] ([Id], [Name], [Description], [FilterId], [OrganisationId], [LastUpdated], [ShortlistAccepted], [IsDeleted]) VALUES (3, N'Competition at Solution Confirmation stage', N'This competition will take you to the solution confirmation page and should contain several shortlisted and non-shortlisted solutions', 2, @organisationId, GETUTCDATE(), NULL, 0)
    INSERT [competitions].[Competitions] ([Id], [Name], [Description], [FilterId], [OrganisationId], [LastUpdated], [ShortlistAccepted], [IsDeleted]) VALUES (4, N'Competition at Task List - Solutions confirmed', N'This competition will take you directly to the task list at the shortlisted solutions stage', 2, @organisationId, GETUTCDATE(), GETUTCDATE(), 0)
    SET IDENTITY_INSERT [competitions].[Competitions] OFF

    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (1, N'10000-001', 0, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (1, N'10000-002', 0, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (1, N'10000-062', 0, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (1, N'10029-003', 0, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (1, N'10030-001', 0, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (1, N'10052-002', 0, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (1, N'99999-89', 0, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (2, N'10000-001', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (2, N'10000-002', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (2, N'10000-062', 0, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (2, N'10029-003', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (2, N'10030-001', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (2, N'10052-002', 0, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (2, N'99999-89', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (3, N'10000-001', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (3, N'10000-002', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (3, N'10000-062', 0, N'Test data')
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (3, N'10029-003', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (3, N'10030-001', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (3, N'10052-002', 0, N'Test data')
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (3, N'99999-89', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (4, N'10000-001', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (4, N'10000-002', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (4, N'10000-062', 0, N'Test data')
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (4, N'10029-003', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (4, N'10030-001', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (4, N'10052-002', 0, N'Test data')
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (4, N'99999-89', 1, NULL)

    INSERT [competitions].[RequiredServices] ([CompetitionId], [SolutionId], [ServiceId]) VALUES (1, N'10000-001', N'10000-001A003')
    INSERT [competitions].[RequiredServices] ([CompetitionId], [SolutionId], [ServiceId]) VALUES (1, N'10000-001', N'10000-001A005')
    INSERT [competitions].[RequiredServices] ([CompetitionId], [SolutionId], [ServiceId]) VALUES (2, N'10000-001', N'10000-001A003')
    INSERT [competitions].[RequiredServices] ([CompetitionId], [SolutionId], [ServiceId]) VALUES (2, N'10000-001', N'10000-001A005')
    INSERT [competitions].[RequiredServices] ([CompetitionId], [SolutionId], [ServiceId]) VALUES (3, N'10000-001', N'10000-001A003')
    INSERT [competitions].[RequiredServices] ([CompetitionId], [SolutionId], [ServiceId]) VALUES (3, N'10000-001', N'10000-001A005')
    INSERT [competitions].[RequiredServices] ([CompetitionId], [SolutionId], [ServiceId]) VALUES (4, N'10000-001', N'10000-001A003')
    INSERT [competitions].[RequiredServices] ([CompetitionId], [SolutionId], [ServiceId]) VALUES (4, N'10000-001', N'10000-001A005')

END
GO
