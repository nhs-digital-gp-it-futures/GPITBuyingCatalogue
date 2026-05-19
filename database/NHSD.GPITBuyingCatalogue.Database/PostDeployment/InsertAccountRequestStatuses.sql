IF NOT EXISTS (SELECT *
               FROM users.AccountRequestStatuses)
    INSERT INTO users.AccountRequestStatuses(Id, [Name])
    VALUES (1, 'Pending'),
           (2, 'Approved'),
           (3, 'Rejected');
GO
