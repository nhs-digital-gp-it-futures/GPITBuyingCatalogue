IF NOT EXISTS (SELECT 1 FROM ordering.[Contracts])
    INSERT INTO ordering.[Contracts] (OrderId)
    SELECT cf.OrderId 
    FROM ordering.ContractFlags cf

    INSERT INTO ordering.[ImplementationPlans] ([ContractId], [IsDefault])
    SELECT c.Id, 0
    FROM ordering.ContractFlags cf
    inner join ordering.[Contracts] c on cf.OrderId = c.OrderId
    inner join ordering.[Orders] o on cf.OrderId = o.Id
    WHERE cf.UseDefaultImplementationPlan = 1
    OR (cf.UseDefaultImplementationPlan = 0 AND o.Completed is not NULL)
GO

IF NOT EXISTS (SELECT 1 FROM ordering.[ContractBilling])
    INSERT INTO ordering.[ContractBilling] ([ContractId], [HasConfirmedRequirements])
    SELECT c.Id, 0
    FROM ordering.ContractFlags cf
    inner join ordering.[Contracts] c on cf.OrderId = c.OrderId
    inner join ordering.[Orders] o on cf.OrderId = o.Id
    WHERE cf.UseDefaultBilling = 1
    OR (cf.UseDefaultBilling = 0 AND o.Completed is not NULL)

    UPDATE ordering.[ContractBilling] SET [HasConfirmedRequirements] = 1
    FROM ordering.ContractFlags cf
    inner join ordering.[Orders] o on cf.OrderId = o.Id
    WHERE cf.HasSpecificRequirements = 0
    OR (cf.HasSpecificRequirements = 1 AND o.Completed is not NULL) 
GO
