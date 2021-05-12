/*-----------------------------------------------------------------------
    Pre deployment placeholder file
------------------------------------------------------------------------*/

-- To delete once data is consistent across all DBs
IF OBJECT_ID('dbo.PricingUnit', 'U') IS NOT NULL
BEGIN
    -- Use the existing 'per organisation' unit in place of
    -- 'per organisation (non-GP practice organisations who consume the care plan service)'
    UPDATE dbo.CataloguePrice
       SET PricingUnitId = '05281ffc-1077-41d5-a253-3077540ef2e9'
     WHERE PricingUnitId = 'f845fcaa-96ae-4884-aeb6-56576699bfcd';

    -- per organisation (non-GP practice organisations who consume the care plan service)
    DELETE FROM dbo.PricingUnit
     WHERE PricingUnitId = 'f845fcaa-96ae-4884-aeb6-56576699bfcd';

    UPDATE dbo.PricingUnit
       SET [Description] = LEFT([Description], 40)
     WHERE LEN([Description]) > 40;

     ALTER TABLE dbo.PricingUnit
    ALTER COLUMN [Description] nvarchar(40) NOT NULL;
END;
GO
