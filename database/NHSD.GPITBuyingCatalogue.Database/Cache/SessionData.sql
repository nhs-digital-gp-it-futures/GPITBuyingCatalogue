CREATE TABLE cache.SessionData
(
    Id nvarchar(449) NOT NULL,
    [Value] varbinary(max) NOT NULL,
    ExpiresAtTime datetimeoffset(7) NOT NULL,
    SlidingExpirationInSeconds bigint NULL,
    AbsoluteExpiration datetimeoffset(7) NULL,
    CONSTRAINT PK_SessionData PRIMARY KEY (Id),
    INDEX IX_SessionData_ExpiresAtTime (ExpiresAtTime),
);
