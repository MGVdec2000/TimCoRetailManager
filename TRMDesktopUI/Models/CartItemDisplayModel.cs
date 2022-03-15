using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDesktopUI.Models
{
    public class CartItemDisplayModel : INotifyPropertyChanged
    {
        public ProductDisplayModel Product { get; set; }
        private int _qtyInCart;

        public int QtyInCart
        {
            get
            {
                return _qtyInCart;
            }
            set
            {
                _qtyInCart = value;
                CallPropertyChanged(nameof(QtyInCart));
                CallPropertyChanged(nameof(DisplayText));
            }
        }

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

        public event PropertyChangedEventHandler PropertyChanged;

        private void CallPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
