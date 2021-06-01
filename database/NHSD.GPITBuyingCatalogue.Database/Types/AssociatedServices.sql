CREATE TYPE import.AssociatedServices AS TABLE
(
    Id nvarchar(14) NOT NULL PRIMARY KEY,
    [Name] nvarchar(255) NOT NULL,
    [Description] nvarchar(1000) NULL,
    OrderGuidance nvarchar(1000) NULL
);
