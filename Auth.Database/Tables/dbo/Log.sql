CREATE TABLE [dbo].[Log]
(
	[Id]			INT					NOT NULL PRIMARY KEY IDENTITY(1,1), 
	[DateTime]		DATETIMEOFFSET		NOT NULL, 
	[AdminUserId]	INT					NULL,
	[UserId]		INT					NULL, 
	[Action]		NVARCHAR(50)		NOT NULL,
	[Text]			NVARCHAR(255)		NULL,
)
