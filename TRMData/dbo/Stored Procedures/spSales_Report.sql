CREATE PROCEDURE [dbo].[spSales_Report]

AS

BEGIN
	SET NOCOUNT ON

	SELECT s.SaleDate, s.SubTotal, s.Tax, s.Total, u.FirstName, u.LastName, u.EmailAddress
	FROM dbo.Sales s
	INNER JOIN dbo.Users u ON s.CashierId = u.Id
END