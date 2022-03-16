using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Controllers
{
    [Authorize]
    public class SalesController : ApiController
    {
    //    [HttpGet]
    //    // GET api/values
    //    public List<ProductModel> Get()
    //    {
    //        ProductData data = new ProductData();

    //        return data.GetProducts();
    //    }

        [HttpPost]
        public void Post(SaleModel sale)
        {
            SaleData data = new SaleData();

            data.SaveSale(sale, RequestContext.Principal.Identity.GetUserId());
        }

        [Route("GetSalesReport")]
        public List<SalesReportModel> GetSalesReport()
        {
            SaleData data = new SaleData();

            return data.GetsSalesReport();
        }
    }
}
