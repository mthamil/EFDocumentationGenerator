CREATE TABLE [dbo].[DerivedChildren]
(
	[Id] [int] NOT NULL,
	[Description] [nvarchar](10) NOT NULL,
	CONSTRAINT [PK_DerivedChildren] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_Children_DerivedChildren] FOREIGN KEY([Id]) REFERENCES [dbo].[Children] ([Id])
)
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'A derived child''s ID.', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'DerivedChildren', @level2type=N'COLUMN', @level2name=N'Id'
GO
