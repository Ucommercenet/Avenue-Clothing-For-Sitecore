using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Project.Transaction.Services;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Transaction.ViewModels;
using Sitecore;
using Sitecore.Commerce.Contacts;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Services.Carts;
using Ucommerce;
using Ucommerce.Api;
using Ucommerce.EntitiesV2;

namespace AvenueClothing.Project.Transaction.Controllers
{
	public class CommerceConnectMiniBasketController : BaseController
    {
	    private readonly ITransactionLibrary _transactionLibrary;
		private readonly IMiniBasketService _miniBasketService;

		public CommerceConnectMiniBasketController(ITransactionLibrary transactionLibrary, IMiniBasketService miniBasketService)
		{
			_transactionLibrary = transactionLibrary;
			_miniBasketService = miniBasketService;
		}

		public ActionResult Rendering()
		{
			var miniBasketViewModel = new MiniBasketRenderingViewModel
			{
				IsEmpty = true,
				RefreshUrl = Url.Action("Refresh")
			};

			if (!_transactionLibrary.HasBasket())
			{
				return View("/Views/MiniBasket/Rendering.cshtml", miniBasketViewModel);
			}

			var cart = GetCart();

			var currency = new Currency()
			{
				ISOCode = cart.CurrencyCode
			};

			miniBasketViewModel.NumberOfItems = (int) cart.Lines.Sum(x => x.Quantity);
			miniBasketViewModel.IsEmpty = miniBasketViewModel.NumberOfItems == 0;
			miniBasketViewModel.Total = cart.Total != null ? new Money(cart.Total.Amount, currency.ISOCode) : new Money(0, currency.ISOCode);

			return View("/Views/MiniBasket/Rendering.cshtml", miniBasketViewModel);
		}

		[HttpGet]
		public ActionResult Refresh()
		{
			return Json(_miniBasketService.Refresh(), JsonRequestBehavior.AllowGet);
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