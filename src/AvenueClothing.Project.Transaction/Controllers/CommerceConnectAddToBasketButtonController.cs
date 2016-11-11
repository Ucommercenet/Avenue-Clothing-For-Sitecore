using System;
using System.Collections.ObjectModel;
using System.Web.Mvc;
using AvenueClothing.Project.Transaction.Services;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Transaction.ViewModels;
using Sitecore;
using Sitecore.Commerce.Contacts;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Entities.Prices;
using Sitecore.Commerce.Services.Carts;
using UCommerce.Api;
using UCommerce.Runtime;
using UCommerce.Transactions;
using Convert = System.Convert;

namespace AvenueClothing.Project.Transaction.Controllers
{
	public class CommerceConnectAddToBasketButtonController : BaseController
	{
		private readonly TransactionLibraryInternal _transactionLibraryInternal;
		private readonly ICatalogContext _catalogContext;
		private readonly IMiniBasketService _miniBasketService;

		public CommerceConnectAddToBasketButtonController(TransactionLibraryInternal transactionLibraryInternal, ICatalogContext catalogContext, IMiniBasketService miniBasketService)
		{
			_transactionLibraryInternal = transactionLibraryInternal;
			_catalogContext = catalogContext;
			_miniBasketService = miniBasketService;
		}

		[HttpGet]
		public ActionResult Rendering()
		{
			var product = _catalogContext.CurrentProduct;

			var viewModel = new AddToBasketButtonRenderingViewModel
			{
				AddToBasketUrl = Url.Action("AddToBasket"),
				BasketUrl = "/cart",
				ConfirmationMessageTimeoutInMillisecs = (int)TimeSpan.FromSeconds(5).TotalMilliseconds,
				ConfirmationMessageClientId = "js-add-to-basket-button-confirmation-message-" + Guid.NewGuid(),
				ProductSku = product.Sku,
				IsProductFamily = product.ProductDefinition.IsProductFamily(),
				Price = CatalogLibrary.CalculatePrice(product).YourPrice.Amount.ToString()
			};
			return View("/Views/AddToBasketButton/Rendering.cshtml", viewModel);
		}

		[HttpPost]
		public ActionResult AddToBasket(AddToBasketButtonAddToBasketViewModel viewModel)
		{
			var cartServiceProvider = new CartServiceProvider();

			var contactFactory = new ContactFactory();
			string userId = contactFactory.GetContact();

			var createCartRequest = new CreateOrResumeCartRequest(Context.GetSiteName(), userId);

			var cart = cartServiceProvider.CreateOrResumeCart(createCartRequest).Cart;

			var cartProduct = new CartProduct
			{
				ProductId = viewModel.ProductSku,
				Price = new Price(Convert.ToDecimal(viewModel.Price), cart.CurrencyCode)
			};

			cartProduct.Properties.Add("VariantSku", viewModel.VariantSku);

			var cartLines = new ReadOnlyCollection<CartLine>(
				new Collection<CartLine>{
					new CartLine
					{
						Product = cartProduct,
						Quantity = (uint) viewModel.Quantity
					}
				}
			);

			var request = new AddCartLinesRequest(cart, cartLines);
			cartServiceProvider.AddCartLines(request);

			return Json(_miniBasketService.Refresh(), JsonRequestBehavior.AllowGet);
		}
	}
}