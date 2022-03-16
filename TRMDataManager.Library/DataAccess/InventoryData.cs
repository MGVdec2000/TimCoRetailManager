using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public class InventoryData
    {
        public object SqlDataAcess { get; private set; }

        public List<InventoryModel> GetInventory()
        {
            SqlDataAccess sql = new SqlDataAccess();

            return sql.LoadData<InventoryModel, dynamic>("dbo.spInventory_GetAll", new { }, "TRMData");
        }

        public int SaveInventory(InventoryModel item)
        {
            SqlDataAccess sql = new SqlDataAccess();

            return sql.SaveData("dbo.spInventory_Insert", item, "TRMData");
        }
    }
}
