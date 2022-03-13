using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDesktopUI.Library.Helpers
{
    public class ConfigHelper : IConfigHelper
    {
        public decimal GetTaxRate()
        {
            decimal output = 0;
            string rateText = ConfigurationManager.AppSettings["taxRate"];
            if (!decimal.TryParse(rateText, out output))
            {
                throw new ConfigurationErrorsException("Could not calculate tax rate! Check app settings.");
            }
            return output;
        }
    }
}
