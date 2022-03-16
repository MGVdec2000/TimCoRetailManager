using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public class SaleData
    {
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            // TODO: Not SOLID/DRY, refactor
            // Start filling in models we will save to the database
            List<SaleDetailDbModel> details = new List<SaleDetailDbModel>();
            ProductData products = new ProductData();
            decimal taxRate = ConfigHelper.GetTaxRate() / 100m;

            foreach (SaleDetailModel item in saleInfo.SaleDetails)
            {
                SaleDetailDbModel detail = new SaleDetailDbModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.QtyInCart,
                };

                // Get information about this product
                var productInfo = products.GetProductById(item.ProductId);
                if (productInfo == null)
                {
                    throw new Exception($"Product ID of { item.ProductId } not found in database");
                }
                detail.PurchasePrice = productInfo.RetailPrice * detail.Quantity;
                if (productInfo.IsTaxable)
                {
                    detail.Tax = detail.PurchasePrice * taxRate;
                }

                details.Add(detail);
            }

            // Create Sale model
            SaleDbModel sale = new SaleDbModel()
            {
                SubTotal = details.Sum(x => x.PurchasePrice),
                Tax = Math.Round(details.Sum(x => x.Tax), 2),
                CashierId = cashierId,
            };
            sale.Total = sale.SubTotal + sale.Tax;
            
            using (SqlDataAccess sql = new SqlDataAccess())
            {
                try
                {
                    sql.StartTransaction("TRMData");

                    sale.Id = sql.SaveDataInTransaction("dbo.spSales_Insert", sale);

                    foreach (var item in details)
                    {
                        item.SaleId = sale.Id;
                        // Save SaleDetailModels
                        sql.SaveDataInTransaction("dbo.spSaleDetails_Insert", item);
                    }

                    sql.CommitTransaction();
                }
                catch
                {
                    sql.RollbackTransaction();
                    throw;
                }
            }
        }
    }
}
