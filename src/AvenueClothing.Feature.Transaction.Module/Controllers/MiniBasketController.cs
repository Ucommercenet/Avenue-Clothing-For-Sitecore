using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Feature.Transaction.Module.ViewModels;
using UCommerce;
using UCommerce.Api;

namespace AvenueClothing.Feature.Transaction.Module.Controllers
{
	public class MiniBasketController : Controller
	{
		public ActionResult MiniBasket()
		{
			var miniBasketViewModel = new MiniBasketViewModel { IsEmpty = true };

			if (!TransactionLibrary.HasBasket())
			{
				return View(miniBasketViewModel);
			}

			var purchaseOrder = TransactionLibrary.GetBasket(false).PurchaseOrder;

			miniBasketViewModel.NumberOfItems = purchaseOrder.OrderLines.Sum(x => x.Quantity);
			miniBasketViewModel.IsEmpty = miniBasketViewModel.NumberOfItems == 0;
			miniBasketViewModel.Total = purchaseOrder.OrderTotal.HasValue ? new Money(purchaseOrder.OrderTotal.Value, purchaseOrder.BillingCurrency) : new Money(0, purchaseOrder.BillingCurrency);

			return View(miniBasketViewModel);
		}
	}
}