CREATE TABLE [catalogue].[CapabilityEpics_History]
(
	[CapabilityId] INT NOT NULL,
    [EpicId] NVARCHAR(10) NOT NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL,
)
