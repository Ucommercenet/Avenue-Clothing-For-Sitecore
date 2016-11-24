using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;
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

		public ActionResult Rendering()
		{
			var cart = GetCart();
			var basketModel = new PurchaseOrderViewModel();

			var currency = new Currency()
			{
				ISOCode = cart.CurrencyCode
			};

			foreach (var cartLine in cart.Lines)
			{
				var orderLineViewModel = new OrderlineViewModel();

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

			basketModel.OrderTotal = new Money(cart.Total.Amount, currency).ToString();
			basketModel.DiscountTotal = new Money(cart.Adjustments.Sum(x => x.Amount), currency).ToString();
			if (cart.Total.TaxTotal != null)
				basketModel.TaxTotal = new Money(cart.Total.TaxTotal.Amount, currency).ToString();
			basketModel.SubTotal = new Money((cart.Total.Amount - cart.Total.TaxTotal.Amount), currency).ToString();

			return View(basketModel);
		}

		[HttpPost]
		public ActionResult Index(PurchaseOrderViewModel model)
		{
			var cartServiceProvider = new CartServiceProvider();
			var cart = GetCart();

			foreach (var orderLine in model.OrderLines)
			{
				var newQuantity = orderLine.Quantity;

				if (model.RemoveOrderlineId == orderLine.OrderLineId)
					newQuantity = 0;

				var bmw = cart.Lines.First(i => i.Product.ProductId == orderLine.Sku);

				bmw.Quantity = (uint)newQuantity;

				if (newQuantity > 0)
				{
					var updateCartLinesRequest = new UpdateCartLinesRequest(cart, new Collection<CartLine> { bmw });
					cartServiceProvider.UpdateCartLines(updateCartLinesRequest);
				}
				else
				{
					var request = new RemoveCartLinesRequest(cart, cart.Lines.Where(l => l.Product.ProductId == bmw.Product.ProductId).ToArray());
					cartServiceProvider.RemoveCartLines(request);
				}
			}

			return Redirect("/Cart");
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
