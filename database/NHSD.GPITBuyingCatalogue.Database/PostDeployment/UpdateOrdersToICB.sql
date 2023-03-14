update ord
SET ord.OrderingPartyId = [ods_organisations].GetParentICB(ord.OrderingPartyId)
FROM 
[GPITBuyingCatalogue].[ordering].[Orders] ord
inner join [GPITBuyingCatalogue].[organisations].[Organisations] o
on ord.OrderingPartyId = o.Id
WHERE 
o.PrimaryRoleId = 'RO98'
