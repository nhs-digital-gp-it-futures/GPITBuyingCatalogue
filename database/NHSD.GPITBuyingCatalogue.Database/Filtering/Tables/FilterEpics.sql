CREATE TABLE filtering.FilterEpics
(
     FilterId int NOT NULL,
     EpicId nvarchar(10) NOT NULL,
     CONSTRAINT PK_FilterEpics PRIMARY KEY (FilterId, EpicId),
     CONSTRAINT FK_FilterEpics_Filter FOREIGN KEY (FilterId) REFERENCES filtering.Filters(Id),
     CONSTRAINT FK_FilterEpics_Epic FOREIGN KEY (EpicId) REFERENCES catalogue.Epics(Id),
);
