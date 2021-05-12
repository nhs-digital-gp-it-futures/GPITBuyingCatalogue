CREATE FUNCTION import.GetSupplierId(@catalogueItemId nvarchar(14))
RETURNS nvarchar(6) AS
BEGIN
    RETURN SUBSTRING(@catalogueItemId, 1, CHARINDEX('-', @catalogueItemId) - 1);
END;
