CREATE TABLE [catalogue].[StandardsCapabilities]
(
    StandardId NVARCHAR(5) NOT NULL,
    CapabilityId INT NOT NULL,
    CONSTRAINT PK_StandardsCapabilities PRIMARY KEY (StandardId, CapabilityId),
    CONSTRAINT FK_StandardsCapabilities_Standard FOREIGN KEY (StandardId) REFERENCES catalogue.Standards(Id) ON DELETE CASCADE,
    CONSTRAINT FK_StandardsCapabilities_Capability FOREIGN KEY (CapabilityId) REFERENCES catalogue.Capabilities(Id) ON DELETE CASCADE,
)
