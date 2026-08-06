DECLARE @DeactivationReasonId int = 0;

IF EXISTS (
    SELECT 1
    FROM [users].[AccountDeactivationReasons]
    WHERE Id = @DeactivationReasonId
)
BEGIN
    UPDATE [users].[AspNetUsers]
    SET DeactivationReasonId = @DeactivationReasonId
    WHERE Disabled = 1
      AND DeactivationReasonId IS NULL;
END;
