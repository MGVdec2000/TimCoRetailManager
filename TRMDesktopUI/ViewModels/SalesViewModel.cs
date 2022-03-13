using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        private BindingList<string> _products;

        public BindingList<string> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }

        private string _itemQuantity;

        public string ItemQuantity
        {
            get { return _itemQuantity; }
            set
            {
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
            }
        }

        public bool CanAddToCart
        {
            get
            {
                bool output = false;

                // Make sure item is selected

                // Make sure quantity is != 0

                return output;
            }
        }
        public void AddToCart()
        {

        }

        public bool CanRemoveFromCart
        {
            get
            {
                bool output = false;

                // Make sure item is selected

                // Make sure quantity is != 0

                return output;
            }
        }
        public void RemoveFromCart()
        {

        }

        private BindingList<string> _cart;

        public BindingList<string> Cart
        {
            get { return _cart; }
            set
            {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }

        public string SubTotal
        {
            get
            {
                // TODO: Replace with Calcuation
                return "$0.00";
            }
        }

        public string Tax
        {
            get
            {
                // TODO: Replace with Calcuation
                return "$0.00";
            }
        }

        public string Total
        {
            get
            {
                // TODO: Replace with Calcuation
                return "$0.00";
            }
        }

        public bool CanCheckout
        {
            get
            {
                bool output = false;

                // Make sure cart is not empty

                return output;
            }
        }
        public void Checkout()
        {

        }

    }
}
