using System;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Transaction.ViewModels;
using Ucommerce;
using Ucommerce.Api;
using Ucommerce.Transactions;

namespace AvenueClothing.Project.Transaction.Controllers
{
	public class ShippingPickerController : BaseController
	{
		private readonly ITransactionLibrary _transactionLibrary;

		public ShippingPickerController(ITransactionLibrary transactionLibrary)
		{
			_transactionLibrary = transactionLibrary;
       }
		public ActionResult Rendering()
		{
			var shipmentPickerViewModel = new ShippingPickerViewModel();

			var basket = _transactionLibrary.GetBasket();
			var shippingCountry = basket.GetAddress(Constants.DefaultShipmentAddressName).Country;

			shipmentPickerViewModel.ShippingCountry = shippingCountry.Name;
			var availableShippingMethods = _transactionLibrary.GetShippingMethods(shippingCountry);

			shipmentPickerViewModel.SelectedShippingMethodId = basket.Shipments.FirstOrDefault() != null
				? basket.Shipments.FirstOrDefault().ShippingMethod.ShippingMethodId : -1;

			foreach (var availableShippingMethod in availableShippingMethods)
			{
				var price = availableShippingMethod.GetPriceForCurrency(basket.BillingCurrency);
				var formattedprice = new Money((price == null ? 0 : price.Price), basket.BillingCurrency.ISOCode);

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
			_transactionLibrary.CreateShipment(createShipmentViewModel.SelectedShippingMethodId, Constants.DefaultShipmentAddressName, true);
			_transactionLibrary.ExecuteBasketPipeline();

			return Redirect("/payment");
		}
	}
}