CREATE TABLE catalogue.PricingUnits
(
    Id smallint IDENTITY(1, 1) NOT NULL,
    [Name] nvarchar(20) NOT NULL,
    TierName nvarchar(30) NOT NULL,
    [Description] nvarchar(100) NOT NULL,
    [Definition] nvarchar(1000) NULL,
    CONSTRAINT PK_PricingUnits PRIMARY KEY (Id),
);
