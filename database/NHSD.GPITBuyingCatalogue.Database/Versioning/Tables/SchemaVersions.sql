CREATE TABLE [versioning].[SchemaVersions]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ScriptName] [nvarchar](255) NOT NULL,
	[Applied] [datetime] NOT NULL,
)
