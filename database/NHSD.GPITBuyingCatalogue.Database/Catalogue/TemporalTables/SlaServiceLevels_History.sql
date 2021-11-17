CREATE TABLE catalogue.SlaServiceLevels_History
(
	Id INT NOT NULl,
    SolutionId NVARCHAR(14) NOT NULL,
    TypeOfService NVARCHAR(100) NOT NULL,
    ServiceLevel NVARCHAR(1000) NOT NULL,
    HowMeasured NVARCHAR(1000) NOT NULL,
    ServiceCredits BIT NOT NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL
);
GO

CREATE CLUSTERED COLUMNSTORE INDEX IX_SlaServiceLevels_History
ON catalogue.SlaServiceLevels_History;
GO

CREATE NONCLUSTERED INDEX IX_SlaServiceLevels_History_Id_Period_Columns
ON catalogue.SlaServiceLevels_History (SysEndTime, SysStartTime, Id);
GO
