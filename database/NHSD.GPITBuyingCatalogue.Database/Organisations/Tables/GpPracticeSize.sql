CREATE TABLE [organisations].[GpPracticeSize]
(
    [OdsCode] NVARCHAR(10) PRIMARY KEY, 
    [NumberOfPatients] INT NOT NULL,
    [ExtractDate] DATETIME2 NOT NULL
)
