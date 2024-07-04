CREATE TABLE filtering.FilterInteroperabilityIntegrationTypes -- TODO: Remove in next release
(
	Id int IDENTITY(1, 1) NOT NULL,
    FilterId int NOT NULL,
    InteroperabilityIntegrationTypeId int NOT NULL,
    CONSTRAINT PK_FilterInteroperabilityIntegrationTypes PRIMARY KEY (Id),
    CONSTRAINT FK_FilterInteroperabilityIntegrationTypes_Filter FOREIGN KEY (FilterId) REFERENCES filtering.Filters(Id),
)
