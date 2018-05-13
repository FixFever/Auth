CREATE TABLE [dbo].[ChangeLog]
(
	[Id]			INT					NOT NULL PRIMARY KEY IDENTITY(1,1), 
	[DateTime]		DATETIMEOFFSET		NOT NULL, 
	[AdminUserId]	INT					NOT NULL,
	[UserId]		INT					NOT NULL, 
	[Text]			NVARCHAR(255)		NOT NULL,
)
