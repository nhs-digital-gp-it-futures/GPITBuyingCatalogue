CREATE TABLE filtering.FilterHostingTypes
(
     Id int IDENTITY(1, 1) NOT NULL,
     FilterId int NOT NULL,
     HostingTypeId int NOT NULL,
     CONSTRAINT PK_FilterHostingTypes PRIMARY KEY (Id),
     CONSTRAINT FK_FilterHostingTypes_Filter FOREIGN KEY (FilterId) REFERENCES filtering.Filters(Id),
);
