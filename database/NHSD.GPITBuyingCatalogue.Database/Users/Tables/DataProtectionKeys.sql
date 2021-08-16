CREATE TABLE users.DataProtectionKeys
(
    Id int IDENTITY(1, 1) NOT NULL,
    FriendlyName nvarchar(max) NULL,
    [Xml] nvarchar(max) NULL,
    CONSTRAINT PK_DataProtectionKeys PRIMARY KEY (Id),
);
