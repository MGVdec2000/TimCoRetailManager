CREATE PROCEDURE [dbo].[spSales_Insert]
	@Id INT OUTPUT,
	@CashierId NVARCHAR(128),
	@SaleDate DATETIME2,
	@SubTotal MONEY,
	@Tax MONEY,
	@Total MONEY
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO dbo.Sales(CashierId, SaleDate, SubTotal, Tax, Total)
	OUTPUT INSERTED.Id
	VALUES(@CashierId, @SaleDate, @SubTotal, @Tax, @Total);

END