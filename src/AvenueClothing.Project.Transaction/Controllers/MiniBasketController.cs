using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Project.Transaction.Services;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Transaction.ViewModels;
using Ucommerce;
using Ucommerce.Api;
using Ucommerce.Transactions;

namespace AvenueClothing.Project.Transaction.Controllers
{
	public class MiniBasketController : BaseController
    {
	    private readonly ITransactionLibrary _transactionLibrary;
		private readonly IMiniBasketService _miniBasketService;

		public MiniBasketController(ITransactionLibrary transactionLibrary, IMiniBasketService miniBasketService)
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
				return View(miniBasketViewModel);
			}

			var purchaseOrder = _transactionLibrary.GetBasket();

			miniBasketViewModel.NumberOfItems = purchaseOrder.OrderLines.Sum(x => x.Quantity);
			miniBasketViewModel.IsEmpty = miniBasketViewModel.NumberOfItems == 0;
			miniBasketViewModel.Total = purchaseOrder.OrderTotal.HasValue ? new Money(purchaseOrder.OrderTotal.Value, purchaseOrder.BillingCurrency) : new Money(0, purchaseOrder.BillingCurrency);

			return View(miniBasketViewModel);
		}

		[HttpGet]
		public ActionResult Refresh()
		{
			return Json(_miniBasketService.Refresh(), JsonRequestBehavior.AllowGet);
		}
	}
}