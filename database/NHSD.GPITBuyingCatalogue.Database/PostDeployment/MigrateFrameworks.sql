IF NOT EXISTS(SELECT 1 FROM catalogue.Frameworks WHERE MaximumTerm IS NOT NULL)
BEGIN
  UPDATE catalogue.Frameworks
    SET MaximumTerm = 36
END
