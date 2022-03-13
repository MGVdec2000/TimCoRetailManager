using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDesktopUI.Library.Models
{
    public class CartItemModel
    {
        public ProductModel Product { get; set; }
        public int QtyInCart { get; set; }
        public string DisplayText
        {
            get
            {
                if (QtyInCart > 1)
                {
                    return $"{ Product.ProductName } ({ QtyInCart })";
                }
                else
                {
                    return Product.ProductName;
                }

            }
        }
    }
}
