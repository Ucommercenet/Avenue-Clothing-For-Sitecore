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

			foreach (var orderLine in cart.Lines)
			{
				var orderLineViewModel = new OrderlineViewModel();

				orderLineViewModel.Quantity = (int)orderLine.Quantity;
				orderLineViewModel.ProductName = orderLine.Product.ProductName;
				orderLineViewModel.Sku = orderLine.Product.ProductId;
				if (orderLine.GetPropertyValue("VariantSku") != null)
				{
					orderLineViewModel.VariantSku = orderLine.GetPropertyValue("VariantSku").ToString();
				}
				orderLineViewModel.Total = new Money(orderLine.Total.Amount, currency).ToString();
				orderLineViewModel.Discount = new Money(orderLine.Adjustments.Sum(x => x.Amount), currency).Value;
				if (orderLine.Total.TaxTotal != null)
					orderLineViewModel.Tax = orderLine.Total.TaxTotal.Amount.ToString();
				orderLineViewModel.Price = new Money(orderLine.Product.Price.Amount, currency).ToString();
				orderLineViewModel.ProductUrl = CatalogLibrary.GetNiceUrlForProduct(CatalogLibrary.GetProduct(orderLine.Product.ProductId));
				orderLineViewModel.PriceWithDiscount = new Money((orderLine.Product.Price.Amount - orderLine.Adjustments.Sum(x => x.Amount)), currency).ToString();
				orderLineViewModel.OrderLineId = Convert.ToInt32(orderLine.ExternalCartLineId);

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

				var updateCartLinesRequest = new UpdateCartLinesRequest(cart, new Collection<CartLine> { bmw });

				var result = cartServiceProvider.UpdateCartLines(updateCartLinesRequest);
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
