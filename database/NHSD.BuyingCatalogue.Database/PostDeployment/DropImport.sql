IF UPPER('$(INCLUDE_IMPORT)') <> 'TRUE'
BEGIN
    DROP PROCEDURE IF EXISTS import.ImportAdditionalService;
    DROP TYPE IF EXISTS import.AdditionalServiceCapability;

    DROP PROCEDURE IF EXISTS import.ImportSolution;
    DROP TYPE IF EXISTS import.SolutionCapability;

    DROP ROLE IF EXISTS Importer;

    DROP SCHEMA IF EXISTS import;
END;
GO
