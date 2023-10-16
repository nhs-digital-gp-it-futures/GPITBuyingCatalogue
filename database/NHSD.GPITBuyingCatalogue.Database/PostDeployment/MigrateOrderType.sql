IF NOT EXISTS(SELECT 1 FROM ordering.Orders WHERE OrderTypeId IS NOT NULL)
BEGIN
  update [ordering].Orders SET OrderTypeId = 1 -- Solution
    where AssociatedServicesOnly = 0

  update [ordering].Orders SET OrderTypeId = 2 -- AssociatedServiceOther
    where AssociatedServicesOnly = 1
END
