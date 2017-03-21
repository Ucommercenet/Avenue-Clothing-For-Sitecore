using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Transaction.Services;
using AvenueClothing.Project.Transaction.ViewModels;
using Sitecore;
using Sitecore.Commerce.Contacts;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Services.Carts;
using UCommerce;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using Convert = System.Convert;

namespace AvenueClothing.Project.Transaction.Controllers
{
	public class CommerceConnectBasketController : BaseController
	{
        private readonly IMiniBasketService _miniBasketService;
	    private readonly ICurrencyFormatingService _currencyFormatingService;

	    public CommerceConnectBasketController(IMiniBasketService miniBasketService, ICurrencyFormatingService currencyFormatingService)
	    {
	        _miniBasketService = miniBasketService;
	        _currencyFormatingService = currencyFormatingService;
	    }

        public ActionResult Rendering()         
		{
			var cart = GetCart();
			var basketModel = new BasketRenderingViewModel();

			var currency = new Currency()
			{
				ISOCode = cart.CurrencyCode
			};

			foreach (var cartLine in cart.Lines)
			{
				var orderLineViewModel = new BasketRenderingViewModel.OrderlineViewModel();

				orderLineViewModel.Quantity = (int)cartLine.Quantity;
				orderLineViewModel.ProductName = cartLine.Product.ProductName;
				orderLineViewModel.Sku = cartLine.Product.ProductId;
				if (cartLine.GetPropertyValue("VariantSku") != null)
				{
					orderLineViewModel.VariantSku = cartLine.GetPropertyValue("VariantSku").ToString();
				}
				orderLineViewModel.Total = new Money(cartLine.Total.Amount, currency).ToString();
				orderLineViewModel.Discount = new Money(cartLine.Adjustments.Sum(x => x.Amount), currency).Value;
				if (cartLine.Total.TaxTotal != null)
					orderLineViewModel.Tax = new Money(cartLine.Total.TaxTotal.Amount, currency).ToString();
				orderLineViewModel.Price = new Money(cartLine.Product.Price.Amount, currency).ToString();
				orderLineViewModel.ProductUrl = CatalogLibrary.GetNiceUrlForProduct(CatalogLibrary.GetProduct(cartLine.Product.ProductId));
				orderLineViewModel.PriceWithDiscount = new Money((cartLine.Product.Price.Amount - cartLine.Adjustments.Sum(x => x.Amount)), currency).ToString();
				orderLineViewModel.OrderLineId = Convert.ToInt32(cartLine.ExternalCartLineId);

				basketModel.OrderLines.Add(orderLineViewModel);
			}
            basketModel.RefreshUrl = Url.Action("UpdateBasket");
            basketModel.RemoveOrderlineUrl = Url.Action("RemoveOrderline");

            basketModel.OrderTotal = new Money(cart.Total.Amount, currency).ToString();
			basketModel.DiscountTotal = new Money(cart.Adjustments.Sum(x => x.Amount), currency).ToString();
			if (cart.Total.TaxTotal != null) 
				basketModel.TaxTotal = new Money(cart.Total.TaxTotal.Amount, currency).ToString();
			basketModel.SubTotal = new Money((cart.Total.Amount - cart.Total.TaxTotal.Amount), currency).ToString();

			return View("/Views/Basket/Rendering.cshtml", basketModel);
		}

        [HttpPost]
        public ActionResult RemoveOrderline(int orderlineId)
        {
            var cartServiceProvider = new CartServiceProvider();
     
            var request = new RemoveCartLinesRequest(GetCart(), GetCart().Lines.Where(line => line.ExternalCartLineId == orderlineId.ToString()));
            cartServiceProvider.RemoveCartLines(request);
          
            return Json(new
            {
                orderlineId = orderlineId
            });
        }

        [HttpPost]
        public ActionResult UpdateBasket(BasketUpdateBasket model)
        {
            var cartServiceProvider = new CartServiceProvider();
            var cart = GetCart();
      
            foreach (var updateOrderline in model.RefreshBasket)
            {
                var newQuantity = updateOrderline.OrderLineQty;
                if (newQuantity <= 0)
                {
                    var request = new RemoveCartLinesRequest(cart, 
                        cart.Lines.Where(line => line.ExternalCartLineId == updateOrderline.OrderLineId.ToString()));
                    cart = cartServiceProvider.RemoveCartLines(request).Cart;
                }
                else
                {
                    var cartLineToUpdate = cart.Lines.First(i => i.ExternalCartLineId == updateOrderline.OrderLineId.ToString());
                    cartLineToUpdate.Quantity = (uint)newQuantity;
                    var updateCartLinesRequest = new UpdateCartLinesRequest(cart, new Collection<CartLine>{cartLineToUpdate});
                    cart = cartServiceProvider.UpdateCartLines(updateCartLinesRequest).Cart;
                }
            }
            
            BasketUpdateBasketViewModel updatedBasket = new BasketUpdateBasketViewModel();
            CultureInfo cultureInfo = CultureInfo.GetCultureInfo(cart.CurrencyCode);

            foreach (var orderLine in cart.Lines)
            {
                var orderLineViewModel = new BasketUpdateOrderline();
                orderLineViewModel.OrderlineId = int.Parse(orderLine.ExternalCartLineId);
                orderLineViewModel.Quantity = (int)orderLine.Quantity;


                orderLineViewModel.Total = _currencyFormatingService.GetFormattedCurrencyString(orderLine.Total.Amount, cultureInfo);
                orderLineViewModel.Discount = orderLine.Adjustments.Sum(x => x.Amount);
                orderLineViewModel.Tax = _currencyFormatingService.GetFormattedCurrencyString(orderLine.Total.TaxTotal.Amount, cultureInfo);
                orderLineViewModel.Price = _currencyFormatingService.GetFormattedCurrencyString(orderLine.Product.Price.Amount, cultureInfo);

                var priceWithDiscount = orderLine.Product.Price.Amount - orderLine.Adjustments.Sum(x => x.Amount);
                orderLineViewModel.PriceWithDiscount = _currencyFormatingService.GetFormattedCurrencyString(priceWithDiscount, cultureInfo);

                updatedBasket.Orderlines.Add(orderLineViewModel);
            }
            
            string orderTotal = _currencyFormatingService.GetFormattedCurrencyString(cart.Total.Amount, cultureInfo);
            string discountTotal = _currencyFormatingService.GetFormattedCurrencyString(cart.Adjustments.Sum(x => x.Amount), cultureInfo);
            string taxTotal = _currencyFormatingService.GetFormattedCurrencyString(cart.Total.TaxTotal.Amount, cultureInfo);
            string subTotal = _currencyFormatingService.GetFormattedCurrencyString(cart.Total.Amount - cart.Total.TaxTotal.Amount, cultureInfo);
      
            updatedBasket.OrderTotal = orderTotal;
            updatedBasket.DiscountTotal = discountTotal;
            updatedBasket.TaxTotal = taxTotal;
            updatedBasket.SubTotal = subTotal;

            return Json(new
            {
                MiniBasketRefresh = _miniBasketService.Refresh(),
                OrderTotal = orderTotal,
                DiscountTotal = discountTotal,
                TaxTotal = taxTotal,
                SubTotal = subTotal,
                OrderLines = updatedBasket.Orderlines
            });
        }

        private Cart GetCart()
        {
            var cartServiceProvider = new CartServiceProvider();

            var contactFactory = new ContactFactory();
            string userId = contactFactory.GetContact();

            var createCartRequest = new CreateOrResumeCartRequest(Context.GetSiteName(), userId);

            return cartServiceProvider.CreateOrResumeCart(createCartRequest).Cart;
        }

	
    }

}
