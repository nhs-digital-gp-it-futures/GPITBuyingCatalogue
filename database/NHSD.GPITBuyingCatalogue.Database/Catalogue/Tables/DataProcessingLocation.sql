CREATE TABLE [catalogue].[DataProcessingLocation]
(
    [DataProcessingInfoId]      INT             NOT NULL PRIMARY KEY,
    [ProcessingLocation]        NVARCHAR(2000)  NOT NULL,
    [AdditionalJurisdiction]    NVARCHAR(2000)  NULL
    CONSTRAINT FK_DataProcessingLocation_DataProcessingInfo FOREIGN KEY ([DataProcessingInfoId]) REFERENCES catalogue.DataProcessingInformation ([Id])
)
