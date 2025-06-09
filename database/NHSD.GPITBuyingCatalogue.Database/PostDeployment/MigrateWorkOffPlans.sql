IF NOT EXISTS(SELECT * FROM catalogue.InProgressSolutionStandards)
BEGIN
    INSERT INTO catalogue.InProgressSolutionStandards([SolutionId], [StandardId])
    SELECT DISTINCT [SolutionId], [StandardId] FROM catalogue.WorkOffPlans
END
