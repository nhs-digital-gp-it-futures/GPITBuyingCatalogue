DECLARE @icbRoleId AS nchar(5) = 'RO261';
DECLARE @wyICBOdsCode AS nchar(3) = 'QWO';
DECLARE @OrganisationId AS int = (SELECT TOP (1) Id FROM organisations.Organisations WHERE PrimaryRoleId = @icbRoleId AND ExternalIdentifier = @wyICBOdsCode);

DECLARE @filters AS TABLE
(
    Id int NOT NULL,
    [Name] nvarchar(255) NOT NULL,
    [Description] nvarchar(255) NOT NULL,
    OrganisationId int NOT NULL,
    FrameworkId NVARCHAR(36) NULL,
    CapabilityId int NOT NULL,
    ClientApplicationTypeId int NULL,
    EpicId nvarchar(10) NULL,
    HostingTypeId int NULL
);

INSERT INTO @filters (Id, [Name], [Description], OrganisationId, FrameworkId, CapabilityId)
VALUES
(1, 'Filter 1', 'GPIT Framework Filter', @OrganisationId, 'NHSDGP001', 1),
(2, 'Filter 2', 'DFOCVC Framework Filter', @OrganisationId, 'DFOCVC001', 2),
(3, 'Filter 3', 'TIF Framework Filter', @OrganisationId, 'TIF001', 3);

INSERT INTO @filters (Id, [Name], [Description], OrganisationId, FrameworkId, CapabilityId, ClientApplicationTypeId, EpicId, HostingTypeId)
VALUES
(4, 'Filter 4', 'Client Application Type Filter', @OrganisationId, 'NHSDGP001', 4, 1, null, null),
(5, 'Filter 5', 'Epic Filter', @OrganisationId, 'DFOCVC001', 5, null, 'C1E1', 1),
(6, 'Filter 6', 'Hosting Type Filter', @OrganisationId, 'TIF001', 6, null, null, 1),
(7, 'Filter 7', 'All Filter', @OrganisationId, 'NHSDGP001', 7, 1, 'C1E1', 1);

SET IDENTITY_INSERT filtering.Filters ON;

MERGE INTO filtering.Filters AS TARGET
     USING @filters AS SOURCE ON TARGET.Id = SOURCE.Id
      WHEN MATCHED THEN
           UPDATE SET
            TARGET.[Name] = SOURCE.[Name],
            TARGET.[Description] = SOURCE.[Description],
            TARGET.OrganisationId = SOURCE.OrganisationId,
            TARGET.FrameworkId = SOURCE.FrameworkId
      WHEN NOT MATCHED BY TARGET THEN
           INSERT (Id, [Name], [Description], OrganisationId, FrameworkId)
           VALUES (SOURCE.Id, SOURCE.[Name], SOURCE.[Description], SOURCE.OrganisationId, SOURCE.FrameworkId);

SET IDENTITY_INSERT filtering.Filters OFF;

MERGE INTO filtering.FilterCapabilities AS TARGET
     USING @filters AS SOURCE
        ON TARGET.FilterId = SOURCE.Id
      WHEN MATCHED THEN
UPDATE SET TARGET.CapabilityId = SOURCE.CapabilityId
      WHEN NOT MATCHED BY TARGET THEN
    INSERT (FilterId, CapabilityId)
    VALUES (SOURCE.Id, SOURCE.CapabilityId);

MERGE INTO filtering.FilterEpics AS TARGET
     USING @filters AS SOURCE
        ON TARGET.FilterId = SOURCE.Id
      WHEN MATCHED THEN
UPDATE SET TARGET.EpicId = SOURCE.EpicId
      WHEN NOT MATCHED BY TARGET AND SOURCE.EpicId IS NOT NULL THEN
    INSERT (FilterId, EpicId)
    VALUES (SOURCE.Id, SOURCE.EpicId);

MERGE INTO filtering.FilterClientApplicationTypes AS TARGET
     USING @filters AS SOURCE
        ON TARGET.FilterId = SOURCE.Id
      WHEN MATCHED THEN
UPDATE SET TARGET.ClientApplicationTypeId = SOURCE.ClientApplicationTypeId
      WHEN NOT MATCHED BY TARGET AND SOURCE.ClientApplicationTypeId IS NOT NULL THEN
    INSERT (FilterId, ClientApplicationTypeId)
    VALUES (SOURCE.Id, SOURCE.ClientApplicationTypeId);

MERGE INTO filtering.FilterHostingTypes AS TARGET
     USING @filters AS SOURCE
        ON TARGET.FilterId = SOURCE.Id
      WHEN MATCHED THEN
UPDATE SET TARGET.HostingTypeId = SOURCE.HostingTypeId
      WHEN NOT MATCHED BY TARGET AND SOURCE.HostingTypeId IS NOT NULL THEN
    INSERT (FilterId, HostingTypeId)
    VALUES (SOURCE.Id, SOURCE.HostingTypeId);
GO
