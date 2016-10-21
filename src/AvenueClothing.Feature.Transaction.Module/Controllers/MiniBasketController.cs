using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Feature.Transaction.Module.ViewModels;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;
using Sitecore.Mvc.Presentation;
using UCommerce;
using UCommerce.Api;
using UCommerce.Transactions;

namespace AvenueClothing.Feature.Transaction.Module.Controllers
{
	public class MiniBasketController : Controller
	{
	    private readonly TransactionLibraryInternal _transactionLibraryInternal;

	    public MiniBasketController(TransactionLibraryInternal transactionLibraryInternal)
	    {
	        _transactionLibraryInternal = transactionLibraryInternal;
	    }

		public ActionResult Rendering()
		{
			var miniBasketViewModel = new MiniBasketViewModel
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
			var viewModel = new RefreshMiniBasketViewModel
			{
				IsEmpty = true
			};

			if (!_transactionLibraryInternal.HasBasket())
			{
				return Json(viewModel, JsonRequestBehavior.AllowGet);
			}

			var purchaseOrder = _transactionLibraryInternal.GetBasket(false).PurchaseOrder;

			var quantity = purchaseOrder.OrderLines.Sum(x => x.Quantity);

			var total = purchaseOrder.OrderTotal.HasValue
				? new Money(purchaseOrder.OrderTotal.Value, purchaseOrder.BillingCurrency)
				: new Money(0, purchaseOrder.BillingCurrency);

			viewModel.NumberOfItems = quantity.ToString();
			viewModel.IsEmpty = quantity == 0;
			viewModel.Total = total.ToString();

			return Json(viewModel, JsonRequestBehavior.AllowGet);
		}
	}
}