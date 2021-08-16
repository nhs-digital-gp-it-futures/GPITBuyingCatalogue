CREATE TABLE ordering.Contacts
(
    Id int IDENTITY(1, 1) NOT NULL,
    FirstName nvarchar(100) NULL, 
    LastName nvarchar(100) NULL,
    Email nvarchar(256) NULL, 
    Phone nvarchar(35) NULL,
    CONSTRAINT PK_Contacts PRIMARY KEY (Id),
);
