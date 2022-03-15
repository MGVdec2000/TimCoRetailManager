using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDesktopUI.Models
{
    public class ProductDisplayModel : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal RetailPrice { get; set; }
        private int _qtyInStock;

        public int QtyInStock
        {
            get
            {
                return _qtyInStock;
            }
            set
            {
                _qtyInStock = value;
                CallPropertyChanged(nameof(QtyInStock));
            }
        }

        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }
        public bool IsTaxable { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void CallPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
