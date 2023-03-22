DECLARE @Milestones TABLE(
	Title NVARCHAR(1000) NOT NULL,
	[Order] INT NOT NULL,
	PaymentTrigger NVARCHAR(1000) NOT NULL
);

DECLARE @AcceptanceCriteria TABLE(
	[Description] NVARCHAR(1000) NOT NULL,
	[Order] INT NOT NULL
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
-- Milestone and Acceptance Criteria Content. Match AC to Milestones
-- By their Order value (1, 2 etc)
------------------------------------------------------------------

INSERT INTO @Milestones
VALUES
('Delivery date (go live)', 1, 'No payment.'),
('Service Stability', 2, 'Charges commence on achievement of delivery date (go live), but payments will not be made until service stability is achieved.');

INSERT INTO @AcceptanceCriteria
VALUES
('The supplier evidences to your satisfaction that the implementation plan includes all deliverables and responsibilities of you, the Service Recipient and the supplier, with appropriate time allocated for these to be met.', 1),
('The supplier evidences to your satisfaction that the Catalogue Solution has been configured to meet the Service Recipient’s operational requirements.', 1),
('The supplier evidences to the Service Recipient’s satisfaction that they’ve met their obligations set out in the Training Standard.', 1),
('Where the supplier is responsible for training, they evidence to the Service Recipient’s satisfaction that end users can use the Catalogue Solution to fulfil relevant business functions.', 1),
('The supplier evidences to the Service Recipient’s satisfaction that the Catalogue Solution can connect to and access national and other interfaces applicable to it.', 1),
('The supplier evidences to the Service Recipient’s satisfaction that any Associated Services ordered that are applicable to implementation have been effectively provided.', 1),
('The supplier evidences to the Service Recipient’s satisfaction that the requirements of the Data Migration Standard and Catalogue Solution Migration Process applicable to the supplier for go live have been met and that the relevant data has migrated to enable the Service Recipient to conduct their relevant business functions effectively.', 1),
('The supplier evidences to your and the Service Recipient’s satisfaction that they will meet their obligations set out in the Service Management Standard.', 1),
('The supplier evidences to your satisfaction that they have appropriate invoicing arrangements in place.', 1),
('Any commercial issues identified to date are visible to both you and the supplier and an agreement on how they are to be handled has been reached between both parties.', 1),
('Your approval that all Milestone 1 activities have been successfully completed.', 1),
('The Service Recipient confirms that the Catalogue Solution is functioning as specified by the supplier and end users can use it effectively.', 2),
('The supplier evidences to your and the Service Recipient’s satisfaction that all of the requirements of the Data Migration Standard and Catalogue Solution Migration Process that are applicable have been met by the supplier, and that all the relevant data has migrated to the Catalogue Solution.', 2),
('The supplier evidences to your and the Service Recipient’s satisfaction that they’re meeting their service management obligations set out in appendix 2 of the Service Management Standard. This must be reasonably demonstrated within 10 working days of achievement of Milestone 1.', 2),
('In relation to Type 2 Catalogue Solutions (which do not need to comply with service levels specified by NHS Digital), the supplier evidences to your and the Service Recipient’s satisfaction that the Catalogue Solution is meeting the applicable service levels.', 2),
('Any commercial issues identified to date are visible to both you and the supplier and an agreement on how they’re to be handled has been reached between both parties.', 2),
('Your approval that all Milestone 1 and 2 activities have been successfully completed.', 2);

IF EXISTS(
	-----------------------------------------------------------------
	-- Finds Current milesstones that have a different title, order,
	-- PaymentTrigger, or have yet to be added
	------------------------------------------------------------------
		SELECT 1
		FROM @Milestones M
		LEFT JOIN ordering.ImplementationPlanMilestones IPM
			ON M.[Order] = IPM.[Order]
			AND M.Title = IPM.Title
			AND M.PaymentTrigger = IPM.PaymentTrigger
		WHERE
		IPM.ImplementationPlanId = @DefaultPlanId
		AND IPM.Id IS NULL

	UNION
	------------------------------------------------------------
	-- Finds if any milestones have been removed from current that
	-- exist in live
	------------------------------------------------------------
		SELECT 1
		FROM ordering.ImplementationPlanMilestones IPM
		LEFT JOIN @Milestones M
			ON M.[Order] = IPM.[Order]
			AND M.Title = IPM.Title
			AND M.PaymentTrigger = IPM.PaymentTrigger
		WHERE
			IPM.ImplementationPlanId = @DefaultPlanId
			AND M.Title IS NULL
	UNION
	----------------------------------------------------------------------
	-- Finds Current Acceptance Critera that has a different Description,
	-- Has moved milestones or doesn't exist
	----------------------------------------------------------------------
		SELECT	1
		FROM @AcceptanceCriteria AC
		LEFT JOIN (
			SELECT
				IPAC.Id,
				IPAC.[Description],
				IPM.[Order]
			FROM Ordering.ImplementationPlanAcceptanceCriteria IPAC
			INNER JOIN Ordering.ImplementationPlanMilestones IPM
				ON IPM.Id = IPAC.ImplementationPlanMilestoneId
			WHERE
				IPM.ImplementationPlanId = @DefaultPlanId			
		) AS IPAC
			ON IPAC.[Description] = AC.[Description]
			AND IPAC.[Order] = AC.[Order]
		WHERE IPAC.Id IS NULL
	UNION
	--------------------------------------------------------------------
	-- Finds if any Acceptance Criteria have been removed from current
	-- that exist in live
	---------------------------------------------------------------------
		SELECT 1
		FROM ordering.ImplementationPlanAcceptanceCriteria IPAC
		INNER JOIN Ordering.ImplementationPlanMilestones IPM
			ON IPM.Id = IPAC.ImplementationPlanMilestoneId
		LEFT JOIN @AcceptanceCriteria AC
			ON IPAC.[Description] = AC.[Description]
			AND IPM.[Order] = AC.[Order]
		WHERE 
			IPM.ImplementationPlanId = @DefaultPlanId
			AND AC.[Description] IS NULL
)
BEGIN
	BEGIN TRY
		BEGIN TRAN

		DECLARE	@MilestoneIds TABLE(
			Id INT NOT NULL,
			[Order] INT NOT NULL
		);

		------------------------------------------------------------
		-- Delete the Acceptance Criteria
		------------------------------------------------------------
		DELETE IPAC
		FROM Ordering.ImplementationPlanAcceptanceCriteria IPAC
		INNER JOIN Ordering.ImplementationPlanMilestones IPM
			ON IPM.Id = IPAC.ImplementationPlanMilestoneId
		WHERE IPM.ImplementationPlanId = @DefaultPlanId;

		------------------------------------------------------------
		-- Delete the Milestones
		------------------------------------------------------------
		DELETE IPM
		FROM Ordering.ImplementationPlanMilestones IPM
		WHERE IPM.ImplementationPlanId = @DefaultPlanId;

		------------------------------------------------------------
		-- Insert new Milestones
		------------------------------------------------------------
		INSERT INTO
		Ordering.ImplementationPlanMilestones
		(ImplementationPlanId, [Order], Title, PaymentTrigger, LastUpdated)
		OUTPUT INSERTED.Id, INSERTED.[Order]
			INTO @MilestoneIds (Id, [Order])
		SELECT
			@DefaultPlanId,
			M.[Order],
			M.Title,
			M.PaymentTrigger,
			GETUTCDATE()
		FROM @Milestones M;

		------------------------------------------------------------
		-- Insert new Acceptance Criteria
		------------------------------------------------------------

		INSERT INTO
		Ordering.ImplementationPlanAcceptanceCriteria
		(ImplementationPlanMilestoneId, [Description], LastUpdated)
		SELECT
			MI.Id,
			AC.[Description],
            GETUTCDATE()
		FROM @AcceptanceCriteria AC
		INNER JOIN @MilestoneIds MI
			ON MI.[Order] = AC.[Order];

		COMMIT TRAN;
	END TRY
	BEGIN CATCH
		ROLLBACK;
		THROW;
	END CATCH
END
