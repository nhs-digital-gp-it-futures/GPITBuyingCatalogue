IF NOT EXISTS (SELECT * FROM ordering.ImplementationPlans WHERE IsDefault = 1)
BEGIN

declare @planId int = 0
declare @milestoneId int = 0

insert ordering.ImplementationPlans (IsDefault, LastUpdated) select 1, GETUTCDATE()

set @planId = @@IDENTITY

insert ordering.ImplementationPlanMilestones (ImplementationPlanId, [Order], Title, PaymentTrigger, LastUpdated) select @planId, 1, 'Milestone 1 (Go Live)', 'No payment.', GETUTCDATE()

set @milestoneId = @@IDENTITY

insert ordering.ImplementationPlanAcceptanceCriteria (ImplementationPlanMilestoneId, [Description]) select @milestoneId, 'The supplier evidences to your satisfaction that the implementation plan includes all deliverables and responsibilities of you, the Service Recipient and the supplier, with appropriate time allocated for these to be met.'
insert ordering.ImplementationPlanAcceptanceCriteria (ImplementationPlanMilestoneId, [Description]) select @milestoneId, 'The supplier evidences to your satisfaction that the Catalogue Solution has been configured to meet the Service Recipient’s operational requirements.'
insert ordering.ImplementationPlanAcceptanceCriteria (ImplementationPlanMilestoneId, [Description]) select @milestoneId, 'The supplier evidences to the Service Recipient’s satisfaction that they’ve met their obligations set out in the Training Standard.'
insert ordering.ImplementationPlanAcceptanceCriteria (ImplementationPlanMilestoneId, [Description]) select @milestoneId, 'Where the supplier is responsible for training, they evidence to the Service Recipient’s satisfaction that end users can use the Catalogue Solution to fulfil relevant business functions.'
insert ordering.ImplementationPlanAcceptanceCriteria (ImplementationPlanMilestoneId, [Description]) select @milestoneId, 'The supplier evidences to the Service Recipient’s satisfaction that the Catalogue Solution can connect to and access national and other interfaces applicable to it.'
insert ordering.ImplementationPlanAcceptanceCriteria (ImplementationPlanMilestoneId, [Description]) select @milestoneId, 'The supplier evidences to the Service Recipient’s satisfaction that any Associated Services ordered that are applicable to implementation have been effectively provided.'
insert ordering.ImplementationPlanAcceptanceCriteria (ImplementationPlanMilestoneId, [Description]) select @milestoneId, 'The supplier evidences to the Service Recipient’s satisfaction that the requirements of the Data Migration Standard and Catalogue Solution Migration Process applicable to the supplier for go live have been met and that the relevant data has migrated to enable the Service Recipient to conduct their relevant business functions effectively.'
insert ordering.ImplementationPlanAcceptanceCriteria (ImplementationPlanMilestoneId, [Description]) select @milestoneId, 'The supplier evidences to your and the Service Recipient’s satisfaction that they will meet their obligations set out in the Service Management Standard.'
insert ordering.ImplementationPlanAcceptanceCriteria (ImplementationPlanMilestoneId, [Description]) select @milestoneId, 'The supplier evidences to your satisfaction that they have appropriate invoicing arrangements in place.'
insert ordering.ImplementationPlanAcceptanceCriteria (ImplementationPlanMilestoneId, [Description]) select @milestoneId, 'Any commercial issues identified to date are visible to both you and the supplier and an agreement on how they are to be handled has been reached between both parties.'
insert ordering.ImplementationPlanAcceptanceCriteria (ImplementationPlanMilestoneId, [Description]) select @milestoneId, 'Your approval that all Milestone 1 activities have been successfully completed.'

insert ordering.ImplementationPlanMilestones (ImplementationPlanId, [Order], Title, PaymentTrigger, LastUpdated) select @planId, 2, 'Milestone 2 (Service Stability)', 'Charges commence on achievement of Milestone 1, but payments will not be made until Milestone 2 is achieved.', GETUTCDATE()

set @milestoneId = @@IDENTITY

insert ordering.ImplementationPlanAcceptanceCriteria (ImplementationPlanMilestoneId, [Description]) select @milestoneId, 'The Service Recipient confirms that the Catalogue Solution is functioning as specified by the supplier and end users can use it effectively.'
insert ordering.ImplementationPlanAcceptanceCriteria (ImplementationPlanMilestoneId, [Description]) select @milestoneId, 'The supplier evidences to your and the Service Recipient’s satisfaction that all of the requirements of the Data Migration Standard and Catalogue Solution Migration Process that are applicable have been met by the supplier, and that all the relevant data has migrated to the Catalogue Solution.'
insert ordering.ImplementationPlanAcceptanceCriteria (ImplementationPlanMilestoneId, [Description]) select @milestoneId, 'The supplier evidences to your and the Service Recipient’s satisfaction that they’re meeting their service management obligations set out in appendix 2 of the Service Management Standard. This must be reasonably demonstrated within 10 working days of achievement of Milestone 1.'
insert ordering.ImplementationPlanAcceptanceCriteria (ImplementationPlanMilestoneId, [Description]) select @milestoneId, 'In relation to Type 2 Catalogue Solutions (which do not need to comply with service levels specified by NHS Digital), the supplier evidences to your and the Service Recipient’s satisfaction that the Catalogue Solution is meeting the applicable service levels.'
insert ordering.ImplementationPlanAcceptanceCriteria (ImplementationPlanMilestoneId, [Description]) select @milestoneId, 'Any commercial issues identified to date are visible to both you and the supplier and an agreement on how they’re to be handled has been reached between both parties.'
insert ordering.ImplementationPlanAcceptanceCriteria (ImplementationPlanMilestoneId, [Description]) select @milestoneId, 'Your approval that all Milestone 1 and 2 activities have been successfully completed.'

END
