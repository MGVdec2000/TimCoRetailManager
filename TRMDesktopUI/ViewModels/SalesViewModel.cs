using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Helpers;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        IProductEndpoint _productEndpoint;
        IConfigHelper _configHelper;
        ISaleEndpoint _saleEndpoint;

        public SalesViewModel(IProductEndpoint productEnpoint, IConfigHelper configHelper, ISaleEndpoint saleEndpoint)
        {
            _productEndpoint = productEnpoint;
            _configHelper = configHelper;
            _saleEndpoint = saleEndpoint;
            _taxRate = _configHelper.GetTaxRate() / 100;
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
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckout);
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
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckout);
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

        decimal _taxRate;
        private void CalculateSubTotal()
        {
            _subtotal = 0;
            _tax = 0;
            foreach (CartItemModel item in Cart)
            {
                _subtotal += item.Product.RetailPrice * item.QtyInCart;
                if (item.Product.IsTaxable)
                {
                    _tax += item.Product.RetailPrice * (decimal)_taxRate;
                }
            }
            _total = _subtotal + Math.Round(_tax, 2);
        }

        private decimal _subtotal = 0;
        public string SubTotal
        {
            get
            {
                CalculateSubTotal();
                return _subtotal.ToString("C");
            }
        }

        private decimal _tax = 0;
        public string Tax
        {
            get
            {
                return _tax.ToString("C");
            }
        }

        public decimal _total = 0;
        public string Total
        {
            get
            {
                return _total.ToString("C");
            }
        }

        public bool CanCheckout
        {
            get
            {
                bool output = false;

                // Make sure cart is not empty
                if (Cart.Count > 0)
                {
                    output = true;
                }

                return output;
            }
        }
        public async Task Checkout()
        {
            // Create SaleModel and post to API
            SaleModel saleModel = new SaleModel();
            foreach (CartItemModel cartItem in Cart)
            {
                saleModel.SaleDetails.Add(
                    new SaleDetailModel()
                        {
                            ProductId = cartItem.Product.Id,
                            QtyInCart = cartItem.QtyInCart
                        });
            }

            await _saleEndpoint.PostSale(saleModel);
        }

    }
}
