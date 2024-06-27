CREATE TABLE filtering.FilterNhsAppIntegrationTypes
(
	Id int IDENTITY(1, 1) NOT NULL,
    FilterId int NOT NULL,
    NhsAppIntegrationsTypeId int NOT NULL,
    CONSTRAINT PK_FilterNhsAppIntegrationsTypes PRIMARY KEY (Id),
    CONSTRAINT FK_FilterNhsAppIntegrationsTypes_Filter FOREIGN KEY (FilterId) REFERENCES filtering.Filters(Id),
)
