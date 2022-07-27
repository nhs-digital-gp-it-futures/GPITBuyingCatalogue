IF NOT EXISTS(SELECT * FROM ordering.DataProcessingPlans WHERE IsDefault = 1)
BEGIN
    INSERT INTO ordering.DataProcessingPlans(IsDefault, LastUpdated) SELECT 1, GETUTCDATE()
END
