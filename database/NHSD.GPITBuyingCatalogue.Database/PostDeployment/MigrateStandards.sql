IF EXISTS(SELECT 1 FROM catalogue.Standards WHERE StandardTypeId IN (2, 3, 4))
BEGIN
  UPDATE catalogue.Standards SET StandardTypeId = 5 WHERE StandardTypeId IN (2, 3, 4)
END
