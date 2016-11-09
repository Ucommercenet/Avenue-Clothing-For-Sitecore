using System;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Feature.Transaction.Module.ViewModels;
using AvenueClothing.Foundation.MvcExtensionsModule;
using UCommerce;
using UCommerce.Transactions;

namespace AvenueClothing.Feature.Transaction.Module.Controllers
{
	public class ShippingPickerController : BaseController
	{
		private readonly TransactionLibraryInternal _transactionLibraryInternal;

		public ShippingPickerController(TransactionLibraryInternal transactionLibraryInternal)
		{
			_transactionLibraryInternal = transactionLibraryInternal;
		}
		public ActionResult Rendering()
		{
			var shipmentPickerViewModel = new ShippingPickerViewModel();

			var basket = _transactionLibraryInternal.GetBasket(false).PurchaseOrder;
			var shippingCountry = basket.GetAddress(Constants.DefaultShipmentAddressName).Country;

			shipmentPickerViewModel.ShippingCountry = shippingCountry.Name;
			var availableShippingMethods = _transactionLibraryInternal.GetShippingMethods(shippingCountry);

			shipmentPickerViewModel.SelectedShippingMethodId = basket.Shipments.FirstOrDefault() != null
				? basket.Shipments.FirstOrDefault().ShippingMethod.ShippingMethodId : -1;

			foreach (var availableShippingMethod in availableShippingMethods)
			{
				var price = availableShippingMethod.GetPriceForCurrency(basket.BillingCurrency);
				var formattedprice = new Money((price == null ? 0 : price.Price), basket.BillingCurrency);

				shipmentPickerViewModel.AvailableShippingMethods.Add(new SelectListItem()
				{
					Selected = shipmentPickerViewModel.SelectedShippingMethodId == availableShippingMethod.ShippingMethodId,
					Text = String.Format(" {0} ({1})", availableShippingMethod.Name, formattedprice),
					Value = availableShippingMethod.ShippingMethodId.ToString()
				});
			}

			return View(shipmentPickerViewModel);
		}

		[HttpPost]
		public ActionResult CreateShipment(ShippingPickerViewModel createShipmentViewModel)
		{
			_transactionLibraryInternal.CreateShipment(createShipmentViewModel.SelectedShippingMethodId, Constants.DefaultShipmentAddressName, true);
			_transactionLibraryInternal.ExecuteBasketPipeline();

			return Redirect("/payment");
		}
	}
}