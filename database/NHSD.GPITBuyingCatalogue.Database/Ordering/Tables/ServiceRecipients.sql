CREATE TABLE ordering.ServiceRecipients
(
    OdsCode nvarchar(8) NOT NULL,
    [Name] nvarchar(256) NULL,
    CONSTRAINT PK_ServiceRecipients PRIMARY KEY (OdsCode),
);
