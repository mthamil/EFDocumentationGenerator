CREATE TABLE [dbo].[Items]
(
	[Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Name] nvarchar(10) NOT NULL,
	[Description] nvarchar(100) NOT NULL,
	[IS_ENABLED] bit NOT NULL CONSTRAINT [DF_Items_IsEnabled] DEFAULT 'FALSE'
)
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'An item''s ID.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Items', @level2type=N'COLUMN',@level2name=N'Id'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'An item''s name.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Items', @level2type=N'COLUMN',@level2name=N'Name'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Whether an item is enabled.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Items', @level2type=N'COLUMN',@level2name='IS_ENABLED'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Represents an item.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Items'
GO
