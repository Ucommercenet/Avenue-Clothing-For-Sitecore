using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Feature.Transaction.Module.Services;
using AvenueClothing.Feature.Transaction.Module.ViewModels;
using AvenueClothing.Foundation.MvcExtensions;
using UCommerce;
using UCommerce.Transactions;

namespace AvenueClothing.Feature.Transaction.Module.Controllers
{
	public class MiniBasketController : BaseController
    {
	    private readonly TransactionLibraryInternal _transactionLibraryInternal;
		private readonly IMiniBasketService _miniBasketService;

		public MiniBasketController(TransactionLibraryInternal transactionLibraryInternal, IMiniBasketService miniBasketService)
		{
			_transactionLibraryInternal = transactionLibraryInternal;
			_miniBasketService = miniBasketService;
		}

		public ActionResult Rendering()
		{
			var miniBasketViewModel = new MiniBasketRenderingViewModel
			{
				IsEmpty = true, 
				RefreshUrl = Url.Action("Refresh")
			};

			if (!_transactionLibraryInternal.HasBasket())
			{
				return View(miniBasketViewModel);
			}

			var purchaseOrder = _transactionLibraryInternal.GetBasket(false).PurchaseOrder;

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