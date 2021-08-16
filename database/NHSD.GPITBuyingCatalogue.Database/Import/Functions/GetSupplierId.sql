CREATE FUNCTION import.GetSupplierId
(
    @catalogueItemId nvarchar(14)
)
RETURNS TABLE
AS
RETURN
(
    SELECT SUBSTRING(@catalogueItemId, 1, CHARINDEX('-', @catalogueItemId) - 1) AS Id
);
