CREATE TABLE filtering.FilterIM1IntegrationTypes
(
	Id int IDENTITY(1, 1) NOT NULL,
    FilterId int NOT NULL,
    IM1IntegrationsTypeId int NOT NULL,
    CONSTRAINT PK_FilterIM1IntegrationsTypes PRIMARY KEY (Id),
    CONSTRAINT FK_FilterIM1IntegrationsTypes_Filter FOREIGN KEY (FilterId) REFERENCES filtering.Filters(Id),
)
