using AutoMapper;
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
using TRMDesktopUI.Models;

namespace TRMDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        IProductEndpoint _productEndpoint;
        IConfigHelper _configHelper;
        ISaleEndpoint _saleEndpoint;
        IMapper _mapper;

        public SalesViewModel(IProductEndpoint productEnpoint, IConfigHelper configHelper,
            ISaleEndpoint saleEndpoint, IMapper mapper)
        {
            _productEndpoint = productEnpoint;
            _configHelper = configHelper;
            _saleEndpoint = saleEndpoint;
            _mapper = mapper;
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
            var products = _mapper.Map<List<ProductDisplayModel>>(productList);
            Products = new BindingList<ProductDisplayModel>(products);
        }

        private BindingList<ProductDisplayModel> _products;

        public BindingList<ProductDisplayModel> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }

        private ProductDisplayModel _selectedProduct;

        public ProductDisplayModel SelectedProduct
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
            CartItemDisplayModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);
            if (existingItem != null)
            {
                existingItem.QtyInCart += ItemQuantity;
            }
            else
            {
                Cart.Add(new CartItemDisplayModel()
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

        private BindingList<CartItemDisplayModel> _cart = new BindingList<CartItemDisplayModel>();

        public BindingList<CartItemDisplayModel> Cart
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
            foreach (CartItemDisplayModel item in Cart)
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
            foreach (CartItemDisplayModel cartItem in Cart)
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
