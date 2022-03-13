using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        IProductEndpoint _productEndpoint;

        public SalesViewModel(IProductEndpoint productEnpoint)
        {
            _productEndpoint = productEnpoint;
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadProducts();
        }

        private async Task LoadProducts()
        {
            var productList = await _productEndpoint.GetAll();
            Products = new BindingList<ProductModel>(productList);
        }

        private BindingList<ProductModel> _products;

        public BindingList<ProductModel> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }

        private ProductModel _selectedProduct;

        public ProductModel SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }


        private int _itemQuantity = 1;

        public int ItemQuantity
        {
            get { return _itemQuantity; }
            set
            {
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        public bool CanAddToCart
        {
            get
            {
                bool output = false;

                // Make sure item is selected
                // Make sure quantity is != 0
                // Make sure quantity on hand is greater than or equal to desired quantity
                if (ItemQuantity > 0 && SelectedProduct?.QtyInStock >= ItemQuantity)
                {
                    output = true;
                }

                return output;
            }
        }
        public void AddToCart()
        {
            CartItemModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);
            if (existingItem != null)
            {
                existingItem.QtyInCart += ItemQuantity;

                // HACK: Seems like there should be a better way
                Cart.Remove(existingItem);
                Cart.Add(existingItem);

            }
            else
            {
                Cart.Add(new CartItemModel()
                {
                    Product = SelectedProduct,
                    QtyInCart = ItemQuantity,
                });
            }
            SelectedProduct.QtyInStock -= ItemQuantity;
            ItemQuantity = 1;
            NotifyOfPropertyChange(() => SubTotal);
        }

        public bool CanRemoveFromCart
        {
            get
            {
                bool output = false;

                // Make sure CartItem is selected
                // Update SelecteItem QtyInStock

                return output;
            }
        }
        public void RemoveFromCart()
        {
            NotifyOfPropertyChange(() => SubTotal);
        }

        private BindingList<CartItemModel> _cart = new BindingList<CartItemModel>();

        public BindingList<CartItemModel> Cart
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
                decimal subtotal = 0;
                foreach (CartItemModel item in Cart)
                {
                    subtotal += item.Product.RetailPrice * item.QtyInCart;
                }
                return subtotal.ToString("C");
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
