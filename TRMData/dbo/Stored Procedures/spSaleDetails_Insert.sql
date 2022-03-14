CREATE PROCEDURE [dbo].[spSaleDetails_Insert]
	@SaleId INT,
	@ProductId INT,
	@PurchasePrice MONEY,
	@Tax MONEY,
	@Quantity INT
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO dbo.SaleDetails(SaleId, ProductId, Quantity, PurchasePrice, Tax)
	VALUES(@SaleId, @ProductId, @Quantity, @PurchasePrice, @Tax)
END
