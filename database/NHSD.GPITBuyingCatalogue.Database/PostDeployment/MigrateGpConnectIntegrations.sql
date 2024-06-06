IF EXISTS (SELECT 1 FROM [catalogue].[Solutions] WHERE Integrations LIKE '%HTML View%' OR Integrations LIKE '%Appointment Booking%' OR Integrations LIKE '%Structured Record%')
BEGIN TRY
    BEGIN TRAN

	    UPDATE [catalogue].[Solutions] SET Integrations = REPLACE(
		    REPLACE(
			    REPLACE(Integrations, 'Structured Record', 'Access Record Structured')
			    , 'Appointment Booking', 'Appointment Management')
			    , 'HTML View', 'Access Record HTML')
	    WHERE Integrations LIKE '%HTML View%' OR Integrations LIKE '%Appointment Booking%' OR Integrations LIKE '%Structured Record%'

	    UPDATE competitions.InteroperabilityCriteria SET Qualifier = CASE
		    WHEN Qualifier = 'Structured Record' THEN 'Access Record Structured'
		    WHEN Qualifier = 'Appointment Booking' THEN 'Appointment Management'
		    WHEN Qualifier = 'HTML View' THEN 'Access Record HTML'
		    ELSE Qualifier
	    END

    COMMIT
END TRY
BEGIN CATCH
    ROLLBACK TRAN
    THROW
END CATCH
