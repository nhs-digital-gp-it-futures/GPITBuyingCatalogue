CREATE TABLE filtering.FilterCapabilities
(
     FilterId int NOT NULL,
     CapabilityId int NOT NULL,
     CONSTRAINT PK_FilterCapabilities PRIMARY KEY (FilterId, CapabilityId),
     CONSTRAINT FK_FilterCapabilities_Filter FOREIGN KEY (FilterId) REFERENCES filtering.Filters(Id),
     CONSTRAINT FK_FilterCapabilities_Capability FOREIGN KEY (CapabilityId) REFERENCES catalogue.Capabilities(Id),
);
