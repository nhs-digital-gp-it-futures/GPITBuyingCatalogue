DELETE FROM ordering.[ContractBilling]
DELETE FROM ordering.[ImplementationPlans] WHERE [IsDefault] = 0
DELETE FROM ordering.[Contracts]
