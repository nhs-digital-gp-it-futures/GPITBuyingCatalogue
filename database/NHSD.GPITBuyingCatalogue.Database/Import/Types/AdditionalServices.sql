CREATE TYPE import.AdditionalServices AS TABLE
(
    Id nvarchar(14) NOT NULL PRIMARY KEY,
    [Name] nvarchar(255) NOT NULL,
    Summary nvarchar(300) NULL,
    [Description] nvarchar(3000) NULL
);
