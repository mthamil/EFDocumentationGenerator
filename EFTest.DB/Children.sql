CREATE TABLE [dbo].[Children]
(
	[Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Name] nvarchar(10) NOT NULL, 
    [ItemId] int NOT NULL, 
    CONSTRAINT [FK_Children_Items] FOREIGN KEY ([ItemId]) REFERENCES [Items]([Id])
)
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'A child''s ID.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Children', @level2type=N'COLUMN',@level2name=N'Id'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'A child''s parent.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Children', @level2type=N'COLUMN',@level2name=N'ItemId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Represents a child item.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Children'
GO