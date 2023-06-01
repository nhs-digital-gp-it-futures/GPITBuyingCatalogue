CREATE TABLE filtering.FilterClientApplicationTypes
(
     Id int IDENTITY(1, 1) NOT NULL,
     FilterId int NOT NULL,
     ClientApplicationTypeId int NOT NULL,
     CONSTRAINT PK_FilterClientApplicationTypes PRIMARY KEY (Id),
     CONSTRAINT FK_FilterClientApplicationTypes_Filter FOREIGN KEY (FilterId) REFERENCES filtering.Filters(Id),
);
