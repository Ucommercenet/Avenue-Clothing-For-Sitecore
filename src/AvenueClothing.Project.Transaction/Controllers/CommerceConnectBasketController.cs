﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Transaction.ViewModels;
using Sitecore;
using Sitecore.Commerce.Contacts;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Services.Carts;
using Ucommerce;
using Ucommerce.Api;
using Ucommerce.EntitiesV2;
using Ucommerce.Search.Slugs;
using Convert = System.Convert;

namespace AvenueClothing.Project.Transaction.Controllers
{
	public class CommerceConnectBasketController : BaseController
	{
		private readonly IUrlService _urlService;
		private readonly ICatalogContext _catalogContext;
		private readonly ICatalogLibrary _catalogLibrary;

		public CommerceConnectBasketController(IUrlService urlService, ICatalogContext catalogContext, ICatalogLibrary catalogLibrary)
		{
			_urlService = urlService;
			_catalogContext = catalogContext;
			_catalogLibrary = catalogLibrary;
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
				orderLineViewModel.Total = new Money(cartLine.Total.Amount, currency.ISOCode).ToString();
				orderLineViewModel.Discount = new Money(cartLine.Adjustments.Sum(x => x.Amount), currency.ISOCode).Value;
				if (cartLine.Total.TaxTotal != null)
					orderLineViewModel.Tax = new Money(cartLine.Total.TaxTotal.Amount, currency.ISOCode).ToString();
				orderLineViewModel.Price = new Money(cartLine.Product.Price.Amount, currency.ISOCode).ToString();
				orderLineViewModel.ProductUrl = _urlService.GetUrl(_catalogContext.CurrentCatalog,
					_catalogLibrary.GetProduct(cartLine.Product.ProductId));
				orderLineViewModel.PriceWithDiscount = new Money((cartLine.Product.Price.Amount - cartLine.Adjustments.Sum(x => x.Amount)), currency.ISOCode).ToString();
				orderLineViewModel.OrderLineId = Convert.ToInt32(cartLine.ExternalCartLineId);

				basketModel.OrderLines.Add(orderLineViewModel);
			}

			basketModel.OrderTotal = new Money(cart.Total.Amount, currency.ISOCode).ToString();
			basketModel.DiscountTotal = new Money(cart.Adjustments.Sum(x => x.Amount), currency.ISOCode).ToString();
			if (cart.Total.TaxTotal != null)
				basketModel.TaxTotal = new Money(cart.Total.TaxTotal.Amount, currency.ISOCode).ToString();
			basketModel.SubTotal = new Money((cart.Total.Amount - cart.Total.TaxTotal.Amount), currency.ISOCode).ToString();

			return View(basketModel);
		}

		[HttpPost]
		public ActionResult Index(BasketRenderingViewModel model)
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
