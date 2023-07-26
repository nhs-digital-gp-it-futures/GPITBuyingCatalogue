DECLARE @Milestones TABLE(
	Title NVARCHAR(1000) NOT NULL,
	[Order] INT NOT NULL,
	PaymentTrigger NVARCHAR(1000) NOT NULL
);

-----------------------------------------------------------------
-- Add Default plan if one doesn't exist
-----------------------------------------------------------------
IF NOT EXISTS(SELECT * FROM ordering.ImplementationPlans WHERE IsDefault = 1)
BEGIN
	INSERT ordering.ImplementationPlans (IsDefault, LastUpdated) 
	VALUES
		(1, GETUTCDATE());
END
-----------------------------------------------------------------
-- Get Default Plan Id
------------------------------------------------------------------

DECLARE @DefaultPlanId INT =			
(SELECT TOP 1
	Id
FROM ordering.ImplementationPlans IP
WHERE
	IP.IsDefault = 1
ORDER BY
	IP.LastUpdated DESC);

-----------------------------------------------------------------
-- Milestone Content.
------------------------------------------------------------------

MERGE INTO ordering.ImplementationPlanMilestones AS TARGET
USING (
VALUES
    (@DefaultPlanId, 'Delivery date (go live)', 1, 'No payment.'),
    (@DefaultPlanId, 'Service Stability', 2, 'Charges commence on achievement of delivery date (go live), but payments will not be made until service stability is achieved.')
)
AS SOURCE ([ImplementationPlanId], [Title], [Order], [PaymentTrigger])
ON TARGET.[Order] = SOURCE.[Order]

WHEN MATCHED 
THEN UPDATE SET
TARGET.[ImplementationPlanId] = SOURCE.[ImplementationPlanId],
TARGET.[Title] = SOURCE.[Title],
TARGET.[PaymentTrigger] = SOURCE.[PaymentTrigger]

WHEN NOT MATCHED BY TARGET THEN
INSERT ([ImplementationPlanId], [Title], [Order], [PaymentTrigger])
VALUES (SOURCE.[ImplementationPlanId], SOURCE.[Title], SOURCE.[Order], SOURCE.[PaymentTrigger]);
GO
