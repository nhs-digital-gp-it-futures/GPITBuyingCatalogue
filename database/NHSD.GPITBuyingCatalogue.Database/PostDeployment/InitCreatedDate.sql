DECLARE @UsersDateMap AS TABLE ([ID] INT, [Created] DATETIME2);

INSERT INTO @UsersDateMap
SELECT Id, MIN(SysStartTime) As Created FROM users.AspNetUsers FOR SYSTEM_TIME ALL GROUP BY ID ORDER BY ID ASC

UPDATE ANU
SET ANU.Created = UDM.Created
FROM users.AspNetUsers ANU
INNER JOIN @UsersDateMap UDM ON ANU.Id = UDM.Id
