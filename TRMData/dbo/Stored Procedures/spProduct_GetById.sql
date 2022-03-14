CREATE PROCEDURE [dbo].[spProduct_GetById]
	@Id int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT Id, ProductName, [Description], RetailPrice, QtyInStock, IsTaxable
	FROM dbo.Products
	WHERE Id = @Id;
END
