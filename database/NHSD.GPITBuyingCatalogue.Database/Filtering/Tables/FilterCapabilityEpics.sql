CREATE TABLE filtering.FilterCapabilityEpics
(
     Id int IDENTITY(1, 1) NOT NULL,
     FilterId int NOT NULL,
     CapabilityId int NOT NULL,
     EpicId nvarchar(10) NULL,
     CONSTRAINT PK_FilterCapabilityEpics PRIMARY KEY (Id),
     CONSTRAINT FK_FilterCapabilityEpics_Filter FOREIGN KEY (FilterId) REFERENCES filtering.Filters(Id),
     CONSTRAINT FK_FilterCapabilityEpics_Capability FOREIGN KEY (CapabilityId) REFERENCES catalogue.Capabilities(Id),
     CONSTRAINT FK_FilterCapabilityEpics_Epic FOREIGN KEY (EpicId) REFERENCES catalogue.Epics(Id),
);
