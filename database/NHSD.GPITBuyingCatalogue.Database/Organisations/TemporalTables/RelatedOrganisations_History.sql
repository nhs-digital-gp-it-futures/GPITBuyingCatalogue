﻿CREATE TABLE organisations.RelatedOrganisations_History
(
    OrganisationId int NOT NULL,
    RelatedOrganisationId int NOT NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL,
);
GO

CREATE CLUSTERED COLUMNSTORE INDEX IX_RelatedOrganisations_History 
ON organisations.RelatedOrganisations_History;
GO

