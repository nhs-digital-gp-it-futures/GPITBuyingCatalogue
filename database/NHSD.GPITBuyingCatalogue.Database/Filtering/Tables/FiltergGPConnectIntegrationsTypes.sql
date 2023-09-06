CREATE TABLE filtering.FiltergGPConnectIntegrationsTypes
(
	Id int IDENTITY(1, 1) NOT NULL,
    FilterId int NOT NULL,
    GPConnectIntegrationsTypeId int NOT NULL,
    CONSTRAINT PK_FilterGPConnectIntegrationsTypes PRIMARY KEY (Id),
    CONSTRAINT FK_FilterGPConnectIntegrationsTypes_Filter FOREIGN KEY (FilterId) REFERENCES filtering.Filters(Id),
)
