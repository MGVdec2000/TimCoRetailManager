using AutoMapper;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _window;

        public SalesViewModel(IProductEndpoint productEnpoint, IConfigHelper configHelper,
            ISaleEndpoint saleEndpoint, IMapper mapper, StatusInfoViewModel status, IWindowManager window)
        {
            _productEndpoint = productEnpoint;
            _configHelper = configHelper;
            _saleEndpoint = saleEndpoint;
            _mapper = mapper;
            _status = status;
            _window = window;
            _taxRate = _configHelper.GetTaxRate() / 100;
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            try
            {
                await LoadProducts();
            }
            catch (Exception ex)
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "System Error";

                if (ex.Message == "Unauthorized")
                {
                    _status.UpdateMessage("Unauthorized", "You do not have permission to access the sales form.");
                    _window.ShowDialog(_status, null, settings);
                }
                else
                {
                    _status.UpdateMessage("Fatal Exception",ex.Message);
                    _window.ShowDialog(_status, null, settings);
                }
                TryClose();

            }
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

        private CartItemDisplayModel _selectedCartItem;

        public CartItemDisplayModel SelectedCartItem
        {
            get { return _selectedCartItem; }
            set
            {
                _selectedCartItem = value;
                _selectedProduct = null;
                NotifyOfPropertyChange(() => SelectedCartItem);
                NotifyOfPropertyChange(() => CanRemoveFromCart);
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
                return SelectedCartItem?.QtyInCart > 0;
            }
        }
        public void RemoveFromCart()
        {
            SelectedCartItem.Product.QtyInStock += 1;
            if (SelectedCartItem.QtyInCart > 1)
            {
                SelectedCartItem.QtyInCart -= 1;
            }
            else
            {
                Cart.Remove(SelectedCartItem);
            }

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckout);
            NotifyOfPropertyChange(() => CanAddToCart);
        }

        private BindingList<CartItemDisplayModel> _cart = new BindingList<CartItemDisplayModel>();

        private async Task ResetSalesViewModel()
        {
            Cart = new BindingList<CartItemDisplayModel>();
            //TODO: Make sure selected cart item is cleared
            await LoadProducts();

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckout);
        }

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

            await ResetSalesViewModel();

        }

    }
}
