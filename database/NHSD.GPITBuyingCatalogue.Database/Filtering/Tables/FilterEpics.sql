CREATE TABLE filtering.FilterEpics
(
     Id int IDENTITY(1, 1) NOT NULL,
     FilterId int NOT NULL,
     CapabilityId int NULL,
     EpicId nvarchar(10) NOT NULL,
     CONSTRAINT PK_FilterEpics PRIMARY KEY (Id),
     CONSTRAINT FK_FilterEpics_Filter FOREIGN KEY (FilterId) REFERENCES filtering.Filters(Id),
     CONSTRAINT FK_FilterEpics_Capability FOREIGN KEY (CapabilityId) REFERENCES catalogue.Capabilities(Id),
     CONSTRAINT FK_FilterEpics_Epic FOREIGN KEY (EpicId) REFERENCES catalogue.Epics(Id),
);
