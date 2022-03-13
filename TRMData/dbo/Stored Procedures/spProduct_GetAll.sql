CREATE PROCEDURE [dbo].[spProduct_GetAll]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT Id, ProductName, [Description], RetailPrice, QtyInStock
	FROM dbo.Products
	ORDER BY dbo.Products.ProductName
END