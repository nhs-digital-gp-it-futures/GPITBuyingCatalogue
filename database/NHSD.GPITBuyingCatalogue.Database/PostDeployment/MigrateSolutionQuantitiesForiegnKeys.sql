BEGIN TRANSACTION

ALTER TABLE [GPITBuyingCatalogue].[competitions].[SolutionQuantities]
DROP CONSTRAINT [FK_SolutionQuantities_Competition];

ALTER TABLE [GPITBuyingCatalogue].[competitions].[SolutionQuantities]
ADD CONSTRAINT [FK_SolutionQuantities_Competition]
FOREIGN KEY ([CompetitionId])
REFERENCES [GPITBuyingCatalogue].[competitions].[Competitions] ([Id])
ON DELETE NO ACTION
ON UPDATE NO ACTION;

ALTER TABLE [GPITBuyingCatalogue].[competitions].[SolutionQuantities]
ADD CONSTRAINT [FK_SolutionQuantities_SublocationRecipient]
FOREIGN KEY ([CompetitionId], [OdsCode])
REFERENCES [GPITBuyingCatalogue].[competitions].[CompetitionSublocationRecipients] ([CompetitionId], [RecipientOdsCode])
ON DELETE CASCADE
ON UPDATE NO ACTION;

COMMIT TRANSACTION;