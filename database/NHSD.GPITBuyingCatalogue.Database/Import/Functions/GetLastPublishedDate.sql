CREATE FUNCTION catalogue.GetLastPublishedDate (@catalogueItemId NVARCHAR(14))
RETURNS DATETIME
WITH EXECUTE AS CALLER
AS
BEGIN
DECLARE @LastPublished DATETIME;
; WITH x AS
(
    SELECT Id, PublishedStatusId, [LastUpdated], LAG(PublishedStatusId,1,-1) OVER (ORDER BY [LastUpdated] ) AS lastValue
    FROM catalogue.CatalogueItems FOR SYSTEM_TIME ALL WHERE Id = @catalogueItemId
) 
SELECT TOP(1) @LastPublished = [LastUpdated] 
FROM x
WHERE PublishedStatusId = 3 and PublishedStatusId <> lastValue ORDER BY LastUpdated DESC
RETURN(@LastPublished)
END;
GO
