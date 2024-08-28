CREATE TABLE [catalogue].[DataProtectionSubProcessors]
(
    [Id]                        INT             NOT NULL PRIMARY KEY,
    [DataProcessingInfoId]      INT             NOT NULL,
    [DataProcessingDetailsId]   INT             NOT NULL,
    [OrganisationName]          NVARCHAR(200)   NOT NULL,
    [PostProcessingPlan]        NVARCHAR(2000)  NOT NULL,
    CONSTRAINT FK_DataProtectionSubProcessors_DataProcessingInformation   FOREIGN KEY ([DataProcessingInfoId]) REFERENCES catalogue.DataProcessingInformation ([Id]),
    CONSTRAINT FK_DataProtectionSubProcessors_DataProcessingDetails       FOREIGN KEY ([DataProcessingDetailsId]) REFERENCES catalogue.DataProcessingDetails ([Id])
)
