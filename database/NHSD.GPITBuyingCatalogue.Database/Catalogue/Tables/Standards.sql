CREATE TABLE [catalogue].[Standards]
(
    [Id] NVARCHAR(5) NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(500) NOT NULL,
    [Url] NVARCHAR(1000) NOT NULL,
    StandardTypeId INT NOT NULL,
    CONSTRAINT PK_Standards PRIMARY KEY (Id),
    CONSTRAINT FK_Standards_StandardType FOREIGN KEY (StandardTypeId) REFERENCES catalogue.StandardTypes(Id),
);
