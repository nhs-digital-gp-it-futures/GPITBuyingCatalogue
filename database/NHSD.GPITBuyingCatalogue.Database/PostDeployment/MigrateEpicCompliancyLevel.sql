IF NOT EXISTS(SELECT 1 FROM catalogue.CapabilityEpics WHERE CompliancyLevelId IS NOT NULL)
  UPDATE ce
    SET ce.CompliancyLevelId = e.CompliancyLevelId  
	FROM catalogue.CapabilityEpics ce
      JOIN catalogue.epics e
	    ON e.CapabilityId = ce.CapabilityId AND e.id = ce.EpicId
GO
