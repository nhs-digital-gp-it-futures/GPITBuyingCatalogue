CREATE TABLE dbo.PricingUnitLookup
(
    Id smallint IDENTITY(1, 1) NOT NULL PRIMARY KEY,
    OriginalId uniqueidentifier UNIQUE,
    [Name] nvarchar(20) UNIQUE,
);
