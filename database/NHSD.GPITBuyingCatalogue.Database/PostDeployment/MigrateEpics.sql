IF NOT EXISTS (SELECT 1 FROM catalogue.CapabilityEpics)
    INSERT INTO catalogue.CapabilityEpics (EpicId, CapabilityId)
    SELECT Id, CapabilityId FROM catalogue.Epics
    WHERE CapabilityId IS NOT NULL
GO
