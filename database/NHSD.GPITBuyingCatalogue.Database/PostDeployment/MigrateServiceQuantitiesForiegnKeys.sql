BEGIN TRANSACTION

ALTER TABLE [GPITBuyingCatalogue].[competitions].[ServiceQuantities]
ADD CONSTRAINT [FK_ServiceQuantities_SublocationRecipient]
FOREIGN KEY ([CompetitionId], [OdsCode])
REFERENCES [GPITBuyingCatalogue].[competitions].[CompetitionSublocationRecipients] ([CompetitionId], [RecipientOdsCode])
ON DELETE CASCADE
ON UPDATE NO ACTION;

COMMIT TRANSACTION;