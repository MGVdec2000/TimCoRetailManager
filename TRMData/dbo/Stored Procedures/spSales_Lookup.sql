CREATE PROCEDURE [dbo].[spSales_Lookup]
	@CashierId NVARCHAR(128),
	@SaleDate DATETIME2
AS
BEGIN
	SET NOCOUNT ON;

	SELECT Id
	FROM dbo.Sales
	WHERE CashierId = @CashierId AND SaleDate = @SaleDate;
END