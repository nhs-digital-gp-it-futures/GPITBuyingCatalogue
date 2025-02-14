CREATE ROLE [BuyingCatalogue];
GO
ALTER ROLE db_datareader ADD MEMBER [BuyingCatalogue];
GO
ALTER ROLE db_datawriter ADD MEMBER [BuyingCatalogue];
GO
GRANT ALTER ON [organisations].[GpPracticeSize] TO [BuyingCatalogue];
GO