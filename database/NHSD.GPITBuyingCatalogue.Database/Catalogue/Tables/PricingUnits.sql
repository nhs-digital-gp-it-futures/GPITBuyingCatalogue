CREATE TABLE catalogue.PricingUnits
(
    Id smallint IDENTITY(1, 1) NOT NULL,
    [Name] nvarchar(20) NOT NULL,
    TierName nvarchar(30) NOT NULL,
    [Description] nvarchar(40) NOT NULL,
    CONSTRAINT PK_PricingUnit PRIMARY KEY (Id),
    CONSTRAINT AK_Name UNIQUE ([Name]),
);
