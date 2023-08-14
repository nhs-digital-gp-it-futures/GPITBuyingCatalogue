IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE'
BEGIN

SET IDENTITY_INSERT [filtering].[Filters] ON

INSERT [filtering].[Filters] ([Id], [Name], [Description], [OrganisationId], [FrameworkId], [Created], [LastUpdated], [LastUpdatedBy], [IsDeleted]) VALUES (1, N'Single result filter', N'This filter should return a single result for EMIS Web GP', 40, NULL, GETUTCDATE(), GETUTCDATE(), 3, 0)
INSERT [filtering].[Filters] ([Id], [Name], [Description], [OrganisationId], [FrameworkId], [Created], [LastUpdated], [LastUpdatedBy], [IsDeleted]) VALUES (2, N'Multiple result filter', N'This filter uses the Productivity Capability to return multiple results. No additional filters have been applied.', 40, NULL, GETUTCDATE(), GETUTCDATE(), 3,  0)
INSERT [filtering].[Filters] ([Id], [Name], [Description], [OrganisationId], [FrameworkId], [Created], [LastUpdated], [LastUpdatedBy], [IsDeleted]) VALUES (3, N'No results filter', N'This filter uses a combination of Capabilities and additional filters to return no results.', 40, NULL, GETUTCDATE(), GETUTCDATE(), 3,  0)

SET IDENTITY_INSERT [filtering].[Filters] OFF

SET IDENTITY_INSERT [filtering].[FilterClientApplicationTypes] ON

INSERT [filtering].[FilterClientApplicationTypes] ([Id], [FilterId], [ClientApplicationTypeId]) VALUES (1, 1, 0)
INSERT [filtering].[FilterClientApplicationTypes] ([Id], [FilterId], [ClientApplicationTypeId]) VALUES (3, 3, 0)
INSERT [filtering].[FilterClientApplicationTypes] ([Id], [FilterId], [ClientApplicationTypeId]) VALUES (4, 3, 2)
INSERT [filtering].[FilterClientApplicationTypes] ([Id], [FilterId], [ClientApplicationTypeId]) VALUES (5, 3, 1)

SET IDENTITY_INSERT [filtering].[FilterClientApplicationTypes] OFF

SET IDENTITY_INSERT [filtering].[FilterHostingTypes] ON

INSERT [filtering].[FilterHostingTypes] ([Id], [FilterId], [HostingTypeId]) VALUES (1, 3, 2)
INSERT [filtering].[FilterHostingTypes] ([Id], [FilterId], [HostingTypeId]) VALUES (2, 3, 3)
INSERT [filtering].[FilterHostingTypes] ([Id], [FilterId], [HostingTypeId]) VALUES (3, 3, 1)
INSERT [filtering].[FilterHostingTypes] ([Id], [FilterId], [HostingTypeId]) VALUES (4, 3, 0)

SET IDENTITY_INSERT [filtering].[FilterHostingTypes] OFF

INSERT [filtering].[FilterCapabilities] ([FilterId], [CapabilityId]) VALUES (1, 1)
INSERT [filtering].[FilterCapabilities] ([FilterId], [CapabilityId]) VALUES (2, 41)
INSERT [filtering].[FilterCapabilities] ([FilterId], [CapabilityId]) VALUES (3, 1)
INSERT [filtering].[FilterCapabilities] ([FilterId], [CapabilityId]) VALUES (3, 5)
INSERT [filtering].[FilterCapabilities] ([FilterId], [CapabilityId]) VALUES (3, 41)

END
