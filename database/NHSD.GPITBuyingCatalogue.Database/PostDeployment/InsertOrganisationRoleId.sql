IF COL_LENGTH('organisations.Organisations', 'OrganisationRoleId') IS NOT NULL 
AND COL_LENGTH('organisations.Organisations', 'PrimaryRoleId') IS NOT NULL
BEGIN
    --Populate OrganisationRoleId column
    UPDATE [GPITBuyingCatalogue].[organisations].[Organisations] SET [OrganisationRoleId] = [PrimaryRoleId]
END;
GO
