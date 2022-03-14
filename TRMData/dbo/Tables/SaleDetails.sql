CREATE TABLE [dbo].[SaleDetails]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[SaleId] INT NOT NULL,
	[ProductId] INT NOT NULL,
	[Quantity] FLOAT NOT NULL,
	[PurchasePrice] MONEY NOT NULL,
	[Tax] MONEY NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_SaleDetails_ToProduct] FOREIGN KEY (ProductId) REFERENCES Products(Id), 
    CONSTRAINT [FK_SaleDetails_ToSales] FOREIGN KEY (SaleId) REFERENCES Sales(Id),
)
