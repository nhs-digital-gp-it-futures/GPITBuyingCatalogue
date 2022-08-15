IF NOT EXISTS(SELECT * FROM users.EmailDomains)
    INSERT INTO users.EmailDomains([Domain])
    VALUES
        ('@nhs.net'),
        ('@nhs.uk'),
        ('@*.nhs.net'),
        ('@*.nhs.uk'),
        ('@bjss.com'),
        ('@sparck.io');
GO
