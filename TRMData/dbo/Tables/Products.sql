CREATE TABLE [dbo].[Products]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[ProductName] NVARCHAR(100) NOT NULL,
	[Description] NVARCHAR(MAX) NOT NULL,
	[RetailPrice] MONEY NOT NULL,
	[QtyInStock] INT NOT NULL DEFAULT 1,
	[CreateDate] DATETIME2(7) NOT NULL DEFAULT getutcdate(),
	[LastModified] DATETIME2(7) NOT NULL DEFAULT getutcdate(),
)
