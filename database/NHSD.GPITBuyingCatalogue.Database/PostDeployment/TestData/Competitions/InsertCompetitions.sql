IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE' AND NOT EXISTS (SELECT * FROM competitions.Competitions)
BEGIN
    DECLARE @organisationId NVARCHAR(3) = (SELECT Id FROM organisations.Organisations WHERE ExternalIdentifier = 'QWO');

    SET IDENTITY_INSERT [competitions].[Competitions] ON

    INSERT [competitions].[Competitions] ([Id], [Name], [Description], [FilterId], [OrganisationId], [LastUpdated], [ShortlistAccepted], [Completed], [IsDeleted], [ContractLength], [IncludesNonPrice], [FrameworkId]) VALUES (1, N'Competition at Solution Selection stage', N'This competition will take you directly to the solution selection stage as no solutions have yet been shortlisted', 2, @organisationId, GETUTCDATE(), NULL, NULL, 0, NULL, NULL, 'TIF001')
    INSERT [competitions].[Competitions] ([Id], [Name], [Description], [FilterId], [OrganisationId], [LastUpdated], [ShortlistAccepted], [Completed], [IsDeleted], [ContractLength], [IncludesNonPrice], [FrameworkId]) VALUES (2, N'Competition at Solution Justification stage', N'This competition will take you to the stage of justifying why you haven''t picked certain solutions', 2, @organisationId, GETUTCDATE(), NULL, NULL, 0, NULL, NULL, 'TIF001')
    INSERT [competitions].[Competitions] ([Id], [Name], [Description], [FilterId], [OrganisationId], [LastUpdated], [ShortlistAccepted], [Completed], [IsDeleted], [ContractLength], [IncludesNonPrice], [FrameworkId]) VALUES (3, N'Competition at Solution Confirmation stage', N'This competition will take you to the solution confirmation page and should contain several shortlisted and non-shortlisted solutions', 2, @organisationId, GETUTCDATE(), NULL, NULL, 0, NULL, NULL, 'TIF001')
    INSERT [competitions].[Competitions] ([Id], [Name], [Description], [FilterId], [OrganisationId], [LastUpdated], [ShortlistAccepted], [Completed], [IsDeleted], [ContractLength], [IncludesNonPrice], [FrameworkId]) VALUES (4, N'Competition at Task List - Solutions confirmed', N'This competition will take you directly to the task list at the shortlisted solutions stage', 2, @organisationId, GETUTCDATE(), GETUTCDATE(), NULL, 0, NULL, NULL, 'TIF001')
    INSERT [competitions].[Competitions] ([Id], [Name], [Description], [FilterId], [OrganisationId], [LastUpdated], [ShortlistAccepted], [Completed], [IsDeleted], [ContractLength], [IncludesNonPrice], [FrameworkId]) VALUES (5, N'Competition at Task List - Recipients Confirmed', N'This competition will take you directly to the task list at the contract length stage', 2, @organisationId, GETUTCDATE(), GETUTCDATE(), NULL, 0, NULL, NULL, 'TIF001')
    INSERT [competitions].[Competitions] ([Id], [Name], [Description], [FilterId], [OrganisationId], [LastUpdated], [ShortlistAccepted], [Completed], [IsDeleted], [ContractLength], [IncludesNonPrice], [FrameworkId]) VALUES (6, N'Competition at Task List - Contract Length Completed', N'This competition will take you directly to the task list at the award criteria stage', 2, @organisationId, GETUTCDATE(), GETUTCDATE(), NULL, 0, 24, NULL, 'TIF001')
    INSERT [competitions].[Competitions] ([Id], [Name], [Description], [FilterId], [OrganisationId], [LastUpdated], [ShortlistAccepted], [Completed], [IsDeleted], [ContractLength], [IncludesNonPrice], [FrameworkId]) VALUES (7, N'Competition at Task List - Price Only', N'This competition will take you directly to the task list for a price only award criteria', 2, @organisationId, GETUTCDATE(), GETUTCDATE(), NULL, 0, 22, 0, 'TIF001')
    INSERT [competitions].[Competitions] ([Id], [Name], [Description], [FilterId], [OrganisationId], [LastUpdated], [ShortlistAccepted], [Completed], [IsDeleted], [ContractLength], [IncludesNonPrice], [FrameworkId]) VALUES (8, N'Competition at Task List - Price and Non-price', N'This competition will take you directly to the task list for a price and non-price award criteria', 2, @organisationId, GETUTCDATE(), GETUTCDATE(), NULL, 0, 15, 1, 'TIF001')
    INSERT [competitions].[Competitions] ([Id], [Name], [Description], [FilterId], [OrganisationId], [LastUpdated], [ShortlistAccepted], [Completed], [IsDeleted], [ContractLength], [IncludesNonPrice], [FrameworkId]) VALUES (9, N'Competition - Interop non-price elements', N'This competition will take you directly to the task list with Interoperability non-price elements', 2, @organisationId, GETUTCDATE(), GETUTCDATE(), NULL, 0, 15, 1, 'TIF001')
    INSERT [competitions].[Competitions] ([Id], [Name], [Description], [FilterId], [OrganisationId], [LastUpdated], [ShortlistAccepted], [Completed], [IsDeleted], [ContractLength], [IncludesNonPrice], [FrameworkId]) VALUES (10, N'Competition - Implementation non-price element', N'This competition will take you directly to the task list with an Implementation non-price element', 2, @organisationId, GETUTCDATE(), GETUTCDATE(), NULL, 0, 15, 1, 'TIF001')
    INSERT [competitions].[Competitions] ([Id], [Name], [Description], [FilterId], [OrganisationId], [LastUpdated], [ShortlistAccepted], [Completed], [IsDeleted], [ContractLength], [IncludesNonPrice], [FrameworkId]) VALUES (11, N'Competition - Service Level non-price element', N'This competition will take you directly to the task list with a Service Level non-price element', 2, @organisationId, GETUTCDATE(), GETUTCDATE(), NULL, 0, 15, 1, 'TIF001')
    INSERT [competitions].[Competitions] ([Id], [Name], [Description], [FilterId], [OrganisationId], [LastUpdated], [ShortlistAccepted], [Completed], [IsDeleted], [ContractLength], [IncludesNonPrice], [FrameworkId]) VALUES (12, N'Competition - All non-price elements', N'This competition will take you directly to the task list with all non-price elements selected', 2, @organisationId, GETUTCDATE(), GETUTCDATE(), NULL, 0, 15, 1, 'TIF001')
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
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (5, N'10000-001', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (5, N'10000-002', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (5, N'10000-062', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (5, N'10029-003', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (5, N'10030-001', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (5, N'10052-002', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (5, N'99999-89', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (6, N'10000-001', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (6, N'10000-002', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (6, N'10000-062', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (6, N'10029-003', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (6, N'10030-001', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (6, N'10052-002', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (6, N'99999-89', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (7, N'10000-001', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (7, N'10000-002', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (7, N'10000-062', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (7, N'10029-003', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (7, N'10030-001', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (7, N'10052-002', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (7, N'99999-89', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (8, N'10000-001', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (8, N'10000-002', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (8, N'10000-062', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (8, N'10029-003', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (8, N'10030-001', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (8, N'10052-002', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (8, N'99999-89', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (9, N'10000-001', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (9, N'10000-002', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (9, N'10000-062', 0, N'Test data')
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (9, N'10029-003', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (9, N'10030-001', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (9, N'10052-002', 0, N'Test data')
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (9, N'99999-89', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (10, N'10000-001', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (10, N'10000-002', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (10, N'10000-062', 0, N'Test data')
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (10, N'10029-003', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (10, N'10030-001', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (10, N'10052-002', 0, N'Test data')
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (10, N'99999-89', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (11, N'10000-001', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (11, N'10000-002', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (11, N'10000-062', 0, N'Test data')
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (11, N'10029-003', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (11, N'10030-001', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (11, N'10052-002', 0, N'Test data')
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (11, N'99999-89', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (12, N'10000-001', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (12, N'10000-002', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (12, N'10000-062', 0, N'Test data')
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (12, N'10029-003', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (12, N'10030-001', 1, NULL)
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (12, N'10052-002', 0, N'Test data')
    INSERT [competitions].[CompetitionSolutions] ([CompetitionId], [SolutionId], [IsShortlisted], [Justification]) VALUES (12, N'99999-89', 1, NULL)

    BEGIN -- Sublocations
        INSERT INTO 
            [competitions].[CompetitionSublocations] ([CompetitionId], [SublocationOdsCode], [OwnerOdsCode]) 
            VALUES (5, '02T', 'QWO'),
                (5,	'03R', 'QWO'),
                (5,	'15F', 'QWO'),
                (5,	'36J', 'QWO'),
                (5,	'X2C4Y', 'QWO'),
                (6,	'02T', 'QWO'),
                (7,	'03R', 'QWO'),
                (8,	'36J', 'QWO'),
                (9,	'03R', 'QWO'),
                (10, '03R', 'QWO'),
                (11, '03R',	'QWO'),
                (12, '03R',	'QWO')
    END
    

    BEGIN -- Service Recipients
        INSERT [competitions].[CompetitionSublocationRecipients] ([CompetitionId], [ParentSublocationOdsCode], [RecipientOdsCode]) VALUES 
        (5,'36J','A99905'),
        (5,'02T','A99930'),
        (5,'36J','A99973'),
        (5,'36J','B82007'),
        (5,'36J','B82053'),
        (5,'36J','B83002'),
        (5,'36J','B83005'),
        (5,'36J','B83008'),
        (5,'36J','B83009'),
        (5,'36J','B83010'),
        (5,'36J','B83012'),
        (5,'36J','B83014'),
        (5,'36J','B83015'),
        (5,'36J','B83016'),
        (5,'36J','B83017'),
        (5,'36J','B83018'),
        (5,'36J','B83019'),
        (5,'36J','B83020'),
        (5,'36J','B83022'),
        (5,'36J','B83023'),
        (5,'36J','B83025'),
        (5,'36J','B83026'),
        (5,'36J','B83028'),
        (5,'36J','B83029'),
        (5,'36J','B83030'),
        (5,'36J','B83031'),
        (5,'36J','B83032'),
        (5,'36J','B83033'),
        (5,'36J','B83034'),
        (5,'36J','B83035'),
        (5,'36J','B83037'),
        (5,'36J','B83038'),
        (5,'36J','B83039'),
        (5,'36J','B83041'),
        (5,'36J','B83042'),
        (5,'36J','B83045'),
        (5,'36J','B83051'),
        (5,'36J','B83052'),
        (5,'36J','B83054'),
        (5,'36J','B83055'),
        (5,'36J','B83056'),
        (5,'36J','B83058'),
        (5,'36J','B83062'),
        (5,'36J','B83063'),
        (5,'36J','B83064'),
        (5,'36J','B83067'),
        (5,'36J','B83602'),
        (5,'36J','B83604'),
        (5,'36J','B83611'),
        (5,'36J','B83614'),
        (5,'36J','B83617'),
        (5,'36J','B83620'),
        (5,'36J','B83621'),
        (5,'36J','B83622'),
        (5,'36J','B83624'),
        (5,'36J','B83626'),
        (5,'36J','B83627'),
        (5,'36J','B83628'),
        (5,'36J','B83629'),
        (5,'36J','B83641'),
        (5,'36J','B83642'),
        (5,'36J','B83653'),
        (5,'36J','B83657'),
        (5,'36J','B83659'),
        (5,'36J','B83660'),
        (5,'36J','B83661'),
        (5,'36J','B83662'),
        (5,'36J','B83666'),
        (5,'36J','B83667'),
        (5,'36J','B83699'),
        (5,'02T','B84001'),
        (5,'02T','B84003'),
        (5,'02T','B84004'),
        (5,'02T','B84005'),
        (5,'02T','B84006'),
        (5,'02T','B84007'),
        (5,'02T','B84008'),
        (5,'02T','B84009'),
        (5,'02T','B84010'),
        (5,'02T','B84011'),
        (5,'02T','B84012'),
        (5,'02T','B84013'),
        (5,'02T','B84014'),
        (5,'02T','B84016'),
        (5,'02T','B84019'),
        (5,'02T','B84021'),
        (5,'02T','B84612'),
        (5,'02T','B84613'),
        (5,'02T','B84618'),
        (5,'02T','B84623'),
        (5,'X2C4Y','B85001'),
        (5,'X2C4Y','B85002'),
        (5,'X2C4Y','B85004'),
        (5,'X2C4Y','B85005'),
        (5,'X2C4Y','B85006'),
        (5,'X2C4Y','B85008'),
        (5,'X2C4Y','B85009'),
        (5,'X2C4Y','B85010'),
        (5,'X2C4Y','B85012'),
        (5,'X2C4Y','B85014'),
        (5,'X2C4Y','B85015'),
        (5,'X2C4Y','B85016'),
        (5,'X2C4Y','B85018'),
        (5,'X2C4Y','B85019'),
        (5,'X2C4Y','B85020'),
        (5,'X2C4Y','B85021'),
        (5,'X2C4Y','B85022'),
        (5,'X2C4Y','B85023'),
        (5,'X2C4Y','B85024'),
        (5,'X2C4Y','B85025'),
        (5,'X2C4Y','B85026'),
        (5,'X2C4Y','B85027'),
        (5,'X2C4Y','B85028'),
        (5,'X2C4Y','B85030'),
        (5,'X2C4Y','B85031'),
        (5,'X2C4Y','B85032'),
        (5,'X2C4Y','B85033'),
        (5,'X2C4Y','B85036'),
        (5,'X2C4Y','B85037'),
        (5,'X2C4Y','B85038'),
        (5,'X2C4Y','B85041'),
        (5,'X2C4Y','B85042'),
        (5,'X2C4Y','B85044'),
        (5,'X2C4Y','B85048'),
        (5,'X2C4Y','B85051'),
        (5,'X2C4Y','B85054'),
        (5,'X2C4Y','B85055'),
        (5,'X2C4Y','B85058'),
        (5,'X2C4Y','B85059'),
        (5,'X2C4Y','B85060'),
        (5,'X2C4Y','B85061'),
        (5,'X2C4Y','B85062'),
        (5,'X2C4Y','B85606'),
        (5,'X2C4Y','B85610'),
        (5,'X2C4Y','B85611'),
        (5,'X2C4Y','B85612'),
        (5,'X2C4Y','B85614'),
        (5,'X2C4Y','B85619'),
        (5,'X2C4Y','B85620'),
        (5,'X2C4Y','B85622'),
        (5,'X2C4Y','B85623'),
        (5,'X2C4Y','B85634'),
        (5,'X2C4Y','B85636'),
        (5,'X2C4Y','B85640'),
        (5,'X2C4Y','B85641'),
        (5,'X2C4Y','B85645'),
        (5,'X2C4Y','B85646'),
        (5,'X2C4Y','B85650'),
        (5,'X2C4Y','B85652'),
        (5,'X2C4Y','B85655'),
        (5,'X2C4Y','B85657'),
        (5,'X2C4Y','B85658'),
        (5,'X2C4Y','B85659'),
        (5,'X2C4Y','B85660'),
        (5,'15F','B86001'),
        (5,'15F','B86002'),
        (5,'15F','B86003'),
        (5,'15F','B86004'),
        (5,'15F','B86005'),
        (5,'15F','B86006'),
        (5,'15F','B86007'),
        (5,'15F','B86008'),
        (5,'15F','B86009'),
        (5,'15F','B86010'),
        (5,'15F','B86011'),
        (5,'15F','B86012'),
        (5,'15F','B86013'),
        (5,'15F','B86014'),
        (5,'15F','B86015'),
        (5,'15F','B86016'),
        (5,'15F','B86017'),
        (5,'15F','B86018'),
        (5,'15F','B86019'),
        (5,'15F','B86020'),
        (5,'15F','B86022'),
        (5,'15F','B86024'),
        (5,'15F','B86025'),
        (5,'15F','B86028'),
        (5,'15F','B86029'),
        (5,'15F','B86030'),
        (5,'15F','B86032'),
        (5,'15F','B86033'),
        (5,'15F','B86034'),
        (5,'15F','B86035'),
        (5,'15F','B86036'),
        (5,'15F','B86038'),
        (5,'15F','B86039'),
        (5,'15F','B86041'),
        (5,'15F','B86042'),
        (5,'15F','B86043'),
        (5,'15F','B86044'),
        (5,'15F','B86048'),
        (5,'15F','B86049'),
        (5,'15F','B86050'),
        (5,'15F','B86051'),
        (5,'15F','B86052'),
        (5,'15F','B86054'),
        (5,'15F','B86055'),
        (5,'15F','B86056'),
        (5,'15F','B86057'),
        (5,'15F','B86058'),
        (5,'15F','B86059'),
        (5,'15F','B86060'),
        (5,'15F','B86061'),
        (5,'15F','B86062'),
        (5,'15F','B86064'),
        (5,'15F','B86066'),
        (5,'15F','B86067'),
        (5,'15F','B86068'),
        (5,'15F','B86069'),
        (5,'15F','B86070'),
        (5,'15F','B86071'),
        (5,'15F','B86075'),
        (5,'15F','B86081'),
        (5,'15F','B86086'),
        (5,'15F','B86089'),
        (5,'15F','B86092'),
        (5,'15F','B86093'),
        (5,'15F','B86094'),
        (5,'15F','B86096'),
        (5,'15F','B86100'),
        (5,'15F','B86101'),
        (5,'15F','B86103'),
        (5,'15F','B86104'),
        (5,'15F','B86106'),
        (5,'15F','B86108'),
        (5,'15F','B86109'),
        (5,'15F','B86110'),
        (5,'15F','B86623'),
        (5,'15F','B86625'),
        (5,'15F','B86642'),
        (5,'15F','B86643'),
        (5,'15F','B86648'),
        (5,'15F','B86654'),
        (5,'15F','B86655'),
        (5,'15F','B86658'),
        (5,'15F','B86666'),
        (5,'15F','B86667'),
        (5,'15F','B86669'),
        (5,'15F','B86672'),
        (5,'15F','B86673'),
        (5,'15F','B86675'),
        (5,'15F','B86678'),
        (5,'15F','B86681'),
        (5,'15F','B86685'),
        (5,'03R','B87001'),
        (5,'03R','B87002'),
        (5,'03R','B87003'),
        (5,'03R','B87004'),
        (5,'03R','B87005'),
        (5,'03R','B87006'),
        (5,'03R','B87007'),
        (5,'03R','B87008'),
        (5,'03R','B87009'),
        (5,'03R','B87011'),
        (5,'03R','B87012'),
        (5,'03R','B87013'),
        (5,'03R','B87015'),
        (5,'03R','B87016'),
        (5,'03R','B87017'),
        (5,'03R','B87018'),
        (5,'03R','B87019'),
        (5,'03R','B87020'),
        (5,'03R','B87021'),
        (5,'03R','B87022'),
        (5,'03R','B87025'),
        (5,'03R','B87026'),
        (5,'03R','B87027'),
        (5,'03R','B87028'),
        (5,'03R','B87030'),
        (5,'03R','B87031'),
        (5,'03R','B87032'),
        (5,'03R','B87033'),
        (5,'03R','B87036'),
        (5,'03R','B87039'),
        (5,'03R','B87041'),
        (5,'03R','B87042'),
        (5,'03R','B87044'),
        (5,'03R','B87600'),
        (5,'03R','B87602'),
        (5,'03R','B87604'),
        (5,'03R','B87616'),
        (5,'X2C4Y','Y00081'),
        (5,'03R','Y00084'),
        (5,'36J','Y00100'),
        (5,'36J','Y00167'),
        (5,'36J','Y00226'),
        (5,'36J','Y00227'),
        (5,'02T','Y00262'),
        (5,'15F','Y00291'),
        (5,'15F','Y00329'),
        (5,'15F','Y00442'),
        (5,'15F','Y00554'),
        (5,'15F','Y00556'),
        (5,'36J','Y00629'),
        (5,'36J','Y00630'),
        (5,'36J','Y00631'),
        (5,'36J','Y00635'),
        (5,'36J','Y00670'),
        (5,'15F','Y00693'),
        (5,'36J','Y00698'),
        (5,'36J','Y00704'),
        (5,'36J','Y00819'),
        (5,'36J','Y00820'),
        (5,'15F','Y00835'),
        (5,'15F','Y00839'),
        (5,'15F','Y00840'),
        (5,'15F','Y00848'),
        (5,'36J','Y00896'),
        (5,'03R','Y01069'),
        (5,'36J','Y01118'),
        (5,'36J','Y01126'),
        (5,'15F','Y01141'),
        (5,'15F','Y01231'),
        (5,'15F','Y01616'),
        (5,'36J','Y01728'),
        (5,'X2C4Y','Y01775'),
        (5,'02T','Y01882'),
        (5,'36J','Y01885'),
        (5,'X2C4Y','Y01909'),
        (5,'15F','Y01912'),
        (5,'X2C4Y','Y01952'),
        (5,'03R','Y01953'),
        (5,'15F','Y02002'),
        (5,'15F','Y02041'),
        (5,'15F','Y02189'),
        (5,'15F','Y02288'),
        (5,'15F','Y02333'),
        (5,'15F','Y02339'),
        (5,'15F','Y02459'),
        (5,'15F','Y02494'),
        (5,'03R','Y02509'),
        (5,'02T','Y02572'),
        (5,'X2C4Y','Y02643'),
        (5,'02T','Y02645'),
        (5,'02T','Y02655'),
        (5,'03R','Y02731'),
        (5,'36J','Y02738'),
        (5,'X2C4Y','Y03012'),
        (5,'X2C4Y','Y03013'),
        (5,'36J','Y03190'),
        (5,'X2C4Y','Y03267'),
        (5,'X2C4Y','Y03268'),
        (5,'15F','Y03322'),
        (5,'36J','Y03391'),
        (5,'02T','Y03418'),
        (5,'15F','Y03554'),
        (5,'15F','Y03561'),
        (5,'15F','Y03564'),
        (5,'03R','Y03604'),
        (5,'02T','Y03655'),
        (5,'X2C4Y','Y03709'),
        (5,'X2C4Y','Y03777'),
        (5,'15F','Y03889'),
        (5,'15F','Y03892'),
        (5,'03R','Y04083'),
        (5,'36J','Y04099'),
        (5,'36J','Y04100'),
        (5,'03R','Y04113'),
        (5,'15F','Y04166'),
        (5,'36J','Y04174'),
        (5,'15F','Y04203'),
        (5,'15F','Y04204'),
        (5,'15F','Y04205'),
        (5,'X2C4Y','Y04266'),
        (5,'36J','Y04346'),
        (5,'36J','Y04347'),
        (5,'36J','Y04348'),
        (5,'36J','Y04349'),
        (5,'36J','Y04350'),
        (5,'36J','Y04351'),
        (5,'36J','Y04352'),
        (5,'15F','Y04377'),
        (5,'36J','Y04378'),
        (5,'36J','Y04391'),
        (5,'36J','Y04392'),
        (5,'36J','Y04393'),
        (5,'36J','Y04394'),
        (5,'15F','Y04482'),
        (5,'15F','Y04567'),
        (5,'15F','Y04572'),
        (5,'X2C4Y','Y04703'),
        (5,'15F','Y04706'),
        (5,'X2C4Y','Y04714'),
        (5,'02T','Y04885'),
        (5,'36J','Y04899'),
        (5,'X2C4Y','Y04918'),
        (5,'X2C4Y','Y05001'),
        (5,'15F','Y05018'),
        (5,'15F','Y05027'),
        (5,'36J','Y05094'),
        (5,'15F','Y05147'),
        (5,'36J','Y05180'),
        (5,'15F','Y05216'),
        (5,'15F','Y05221'),
        (5,'03R','Y05305'),
        (5,'03R','Y05306'),
        (5,'X2C4Y','Y05371'),
        (5,'36J','Y05397'),
        (5,'36J','Y05398'),
        (5,'15F','Y05404'),
        (5,'03R','Y05409'),
        (5,'36J','Y05483'),
        (5,'36J','Y05484'),
        (5,'02T','Y05620'),
        (5,'15F','Y05623'),
        (5,'03R','Y05737'),
        (5,'02T','Y05740'),
        (5,'36J','Y05776'),
        (5,'15F','Y05777'),
        (5,'15F','Y05793'),
        (5,'36J','Y05798'),
        (5,'36J','Y05799'),
        (5,'36J','Y05800'),
        (5,'36J','Y05801'),
        (5,'36J','Y05802'),
        (5,'36J','Y05803'),
        (5,'15F','Y05805'),
        (5,'36J','Y05806'),
        (5,'36J','Y05807'),
        (5,'36J','Y05808'),
        (5,'36J','Y05809'),
        (5,'36J','Y05811'),
        (5,'36J','Y05812'),
        (5,'03R','Y05819'),
        (5,'36J','Y05823'),
        (5,'15F','Y05863'),
        (5,'15F','Y05864'),
        (5,'36J','Y05875'),
        (5,'36J','Y05903'),
        (5,'03R','Y05959'),
        (5,'X2C4Y','Y05974'),
        (5,'15F','Y05992'),
        (5,'15F','Y05993'),
        (5,'15F','Y05999'),
        (5,'15F','Y06097'),
        (5,'15F','Y06098'),
        (5,'15F','Y06102'),
        (5,'15F','Y06109'),
        (5,'15F','Y06110'),
        (5,'15F','Y06111'),
        (5,'36J','Y06140'),
        (5,'15F','Y06272'),
        (5,'X2C4Y','Y06362'),
        (5,'36J','Y06429'),
        (5,'03R','Y06477'),
        (5,'X2C4Y','Y06483'),
        (5,'02T','Y06490'),
        (5,'02T','Y06491'),
        (5,'02T','Y06492'),
        (5,'02T','Y06493'),
        (5,'15F','Y06504'),
        (5,'X2C4Y','Y06506'),
        (5,'36J','Y06517'),
        (5,'36J','Y06608'),
        (5,'03R','Y06624'),
        (5,'03R','Y06625'),
        (5,'X2C4Y','Y06659'),
        (5,'03R','Y06716'),
        (5,'36J','Y06722'),
        (5,'15F','Y06735'),
        (5,'36J','Y06737'),
        (5,'15F','Y06768'),
        (5,'X2C4Y','Y06801'),
        (5,'36J','Y06862'),
        (5,'15F','Y06876'),
        (5,'02T','Y06896'),
        (5,'15F','Y06939'),
        (5,'15F','Y06940'),
        (5,'15F','Y06941'),
        (5,'03R','Y06946'),
        (5,'36J','Y06992'),
        (5,'15F','Y07036'),
        (5,'03R','Y07122'),
        (5,'15F','Y07123'),
        (5,'36J','Y07199'),
        (5,'X2C4Y','Y07201'),
        (5,'X2C4Y','Y07206'),
        (5,'X2C4Y','Y07207'),
        (5,'X2C4Y','Y07208'),
        (5,'36J','Y07225'),
        (5,'X2C4Y','Y07271'),
        (5,'15F','Y07272'),
        (5,'X2C4Y','Y07275'),
        (5,'36J','Y07306'),
        (5,'X2C4Y','Y07460'),
        (5,'15F','Y07470'),
        (5,'15F','Y07692'),
        (5,'15F','Y07718'),
        (5,'15F','Y07789'),
        (6,'02T','A99930'),
        (6,'02T','B84001'),
        (6,'02T','B84003'),
        (6,'02T','B84004'),
        (6,'02T','B84005'),
        (6,'02T','B84006'),
        (6,'02T','B84007'),
        (6,'02T','B84008'),
        (6,'02T','B84009'),
        (6,'02T','B84010'),
        (6,'02T','B84011'),
        (6,'02T','B84012'),
        (6,'02T','B84013'),
        (6,'02T','B84014'),
        (6,'02T','B84016'),
        (6,'02T','B84019'),
        (6,'02T','B84021'),
        (6,'02T','B84612'),
        (6,'02T','B84613'),
        (6,'02T','B84618'),
        (6,'02T','B84623'),
        (6,'02T','Y00262'),
        (6,'02T','Y01882'),
        (6,'02T','Y02572'),
        (6,'02T','Y02645'),
        (6,'02T','Y02655'),
        (6,'02T','Y03418'),
        (6,'02T','Y03655'),
        (6,'02T','Y04885'),
        (6,'02T','Y05620'),
        (6,'02T','Y05740'),
        (6,'02T','Y06490'),
        (6,'02T','Y06491'),
        (6,'02T','Y06492'),
        (6,'02T','Y06493'),
        (6,'02T','Y06896'),
        (7,'03R','B87003'),
        (7,'03R','B87020'),
        (7,'03R','B87021'),
        (7,'03R','B87025'),
        (7,'03R','B87044'),
        (7,'03R','Y01953'),
        (8,'36J','A99905'),
        (8,'36J','B83062'),
        (8,'36J','B83620'),
        (8,'36J','B83641'),
        (8,'36J','Y04346'),
        (8,'36J','Y04347'),
        (8,'36J','Y05798'),
        (8,'36J','Y05799'),
        (8,'36J','Y06862'),
        (8,'36J','Y07199'),
        (9,'03R','B87003'),
        (9,'03R','B87020'),
        (9,'03R','B87021'),
        (9,'03R','B87025'),
        (9,'03R','B87044'),
        (9,'03R','Y01953'),
        (10,'03R','B87003'),
        (10,'03R','B87020'),
        (10,'03R','B87021'),
        (10,'03R','B87025'),
        (10,'03R','B87044'),
        (10,'03R','Y01953'),
        (11,'03R','B87003'),
        (11,'03R','B87020'),
        (11,'03R','B87021'),
        (11,'03R','B87025'),
        (11,'03R','B87044'),
        (11,'03R','Y01953'),
        (12,'03R','B87003'),
        (12,'03R','B87020'),
        (12,'03R','B87021'),
        (12,'03R','B87025'),
        (12,'03R','B87044'),
        (12,'03R','Y01953')
    END

    INSERT [competitions].[SolutionServices] ([CompetitionId], [SolutionId], [ServiceId], [IsRequired]) VALUES (1, N'10000-001', N'10000-001A003', 1)
    INSERT [competitions].[SolutionServices] ([CompetitionId], [SolutionId], [ServiceId], [IsRequired]) VALUES (1, N'10000-001', N'10000-001A005', 1)
    INSERT [competitions].[SolutionServices] ([CompetitionId], [SolutionId], [ServiceId], [IsRequired]) VALUES (2, N'10000-001', N'10000-001A003', 1)
    INSERT [competitions].[SolutionServices] ([CompetitionId], [SolutionId], [ServiceId], [IsRequired]) VALUES (2, N'10000-001', N'10000-001A005', 1)
    INSERT [competitions].[SolutionServices] ([CompetitionId], [SolutionId], [ServiceId], [IsRequired]) VALUES (3, N'10000-001', N'10000-001A003', 1)
    INSERT [competitions].[SolutionServices] ([CompetitionId], [SolutionId], [ServiceId], [IsRequired]) VALUES (3, N'10000-001', N'10000-001A005', 1)
    INSERT [competitions].[SolutionServices] ([CompetitionId], [SolutionId], [ServiceId], [IsRequired]) VALUES (4, N'10000-001', N'10000-001A003', 1)
    INSERT [competitions].[SolutionServices] ([CompetitionId], [SolutionId], [ServiceId], [IsRequired]) VALUES (4, N'10000-001', N'10000-001A005', 1)
    INSERT [competitions].[SolutionServices] ([CompetitionId], [SolutionId], [ServiceId], [IsRequired]) VALUES (5, N'10000-001', N'10000-001A003', 1)
    INSERT [competitions].[SolutionServices] ([CompetitionId], [SolutionId], [ServiceId], [IsRequired]) VALUES (5, N'10000-001', N'10000-001A005', 1)
    INSERT [competitions].[SolutionServices] ([CompetitionId], [SolutionId], [ServiceId], [IsRequired]) VALUES (6, N'10000-001', N'10000-001A003', 1)
    INSERT [competitions].[SolutionServices] ([CompetitionId], [SolutionId], [ServiceId], [IsRequired]) VALUES (6, N'10000-001', N'10000-001A005', 1)
    INSERT [competitions].[SolutionServices] ([CompetitionId], [SolutionId], [ServiceId], [IsRequired]) VALUES (7, N'10000-001', N'10000-001A003', 1)
    INSERT [competitions].[SolutionServices] ([CompetitionId], [SolutionId], [ServiceId], [IsRequired]) VALUES (7, N'10000-001', N'10000-001A005', 1)
    INSERT [competitions].[SolutionServices] ([CompetitionId], [SolutionId], [ServiceId], [IsRequired]) VALUES (8, N'10000-001', N'10000-001A003', 1)
    INSERT [competitions].[SolutionServices] ([CompetitionId], [SolutionId], [ServiceId], [IsRequired]) VALUES (8, N'10000-001', N'10000-001A005', 1)
    INSERT [competitions].[SolutionServices] ([CompetitionId], [SolutionId], [ServiceId], [IsRequired]) VALUES (9, N'10000-001', N'10000-001A003', 1)
    INSERT [competitions].[SolutionServices] ([CompetitionId], [SolutionId], [ServiceId], [IsRequired]) VALUES (9, N'10000-001', N'10000-001A005', 1)
    INSERT [competitions].[SolutionServices] ([CompetitionId], [SolutionId], [ServiceId], [IsRequired]) VALUES (10, N'10000-001', N'10000-001A003', 1)
    INSERT [competitions].[SolutionServices] ([CompetitionId], [SolutionId], [ServiceId], [IsRequired]) VALUES (10, N'10000-001', N'10000-001A005', 1)
    INSERT [competitions].[SolutionServices] ([CompetitionId], [SolutionId], [ServiceId], [IsRequired]) VALUES (11, N'10000-001', N'10000-001A003', 1)
    INSERT [competitions].[SolutionServices] ([CompetitionId], [SolutionId], [ServiceId], [IsRequired]) VALUES (11, N'10000-001', N'10000-001A005', 1)
    INSERT [competitions].[SolutionServices] ([CompetitionId], [SolutionId], [ServiceId], [IsRequired]) VALUES (12, N'10000-001', N'10000-001A005', 1)
    INSERT [competitions].[SolutionServices] ([CompetitionId], [SolutionId], [ServiceId], [IsRequired]) VALUES (12, N'10000-001', N'10000-001A003', 1)

    INSERT [competitions].[Weightings] ([CompetitionId], [NonPrice], [Price]) VALUES (9, 50, 50), (10, 45, 55), (11, 50, 50), (12, 50, 50);
    
    SET IDENTITY_INSERT [competitions].[NonPriceElements] ON
    INSERT [competitions].[NonPriceElements] ([Id], [CompetitionId]) VALUES (0, 9)
    INSERT [competitions].[NonPriceElements] ([Id], [CompetitionId]) VALUES (1, 10)
    INSERT [competitions].[NonPriceElements] ([Id], [CompetitionId]) VALUES (2, 11)
    INSERT [competitions].[NonPriceElements] ([Id], [CompetitionId]) VALUES (3, 12)
    SET IDENTITY_INSERT [competitions].[NonPriceElements] OFF

    SET IDENTITY_INSERT [competitions].[ImplementationCriteria] ON
    INSERT [competitions].[ImplementationCriteria] ([Id], [Requirements], [NonPriceElementsId]) VALUES (1, N'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua', 1)
    INSERT [competitions].[ImplementationCriteria] ([Id], [Requirements], [NonPriceElementsId]) VALUES (2, N'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua', 3)
    SET IDENTITY_INSERT [competitions].[ImplementationCriteria] OFF

    SET IDENTITY_INSERT [competitions].[ServiceLevelCriteria] ON
    INSERT [competitions].[ServiceLevelCriteria] ([Id], [TimeFrom], [TimeUntil], [ApplicableDays], [NonPriceElementsId]) VALUES (1, GETUTCDATE(), GETUTCDATE(), N'0,1,2,3,4', 2)
    INSERT [competitions].[ServiceLevelCriteria] ([Id], [TimeFrom], [TimeUntil], [ApplicableDays], [NonPriceElementsId]) VALUES (2, GETUTCDATE(), GETUTCDATE(), N'0,1,2,3,4', 3)
    SET IDENTITY_INSERT [competitions].[ServiceLevelCriteria] OFF
    
    INSERT [competitions].[IntegrationsCriteria] ([NonPriceElementsId],[IntegrationTypeId]) VALUES (0, 5)
    INSERT [competitions].[IntegrationsCriteria] ([NonPriceElementsId],[IntegrationTypeId]) VALUES (0, 6)
    INSERT [competitions].[IntegrationsCriteria] ([NonPriceElementsId],[IntegrationTypeId]) VALUES (0, 3)
    INSERT [competitions].[IntegrationsCriteria] ([NonPriceElementsId],[IntegrationTypeId]) VALUES (3, 3)
    INSERT [competitions].[IntegrationsCriteria] ([NonPriceElementsId],[IntegrationTypeId]) VALUES (3, 5)
    INSERT [competitions].[IntegrationsCriteria] ([NonPriceElementsId],[IntegrationTypeId]) VALUES (3, 6)

END
GO
