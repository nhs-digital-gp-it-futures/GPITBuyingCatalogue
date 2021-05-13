CREATE TYPE import.Solutions AS TABLE
(
    Id nvarchar(14) NOT NULL PRIMARY KEY,
    [Name] nvarchar(255) NOT NULL,
    IsFoundation bit DEFAULT 0 NOT NULL,
    FrameworkId nvarchar(10) NOT NULL
);
