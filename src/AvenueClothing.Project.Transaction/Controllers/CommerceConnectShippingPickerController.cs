using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Transaction.ViewModels;
using Sitecore;
using Sitecore.Commerce.Contacts;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Entities.Shipping;
using Sitecore.Commerce.Services.Carts;
using Sitecore.Commerce.Services.Shipping;
using Sitecore.Configuration;
using UCommerce;
using UCommerce.Transactions;
using Constants = UCommerce.Constants;

namespace AvenueClothing.Project.Transaction.Controllers
{
	public class CommerceConnectShippingPickerController : BaseController
	{
		private readonly TransactionLibraryInternal _transactionLibraryInternal;

		public CommerceConnectShippingPickerController(TransactionLibraryInternal transactionLibraryInternal)
		{
			_transactionLibraryInternal = transactionLibraryInternal;
		}

		public ActionResult Rendering()
		{
			var shipmentPickerViewModel = new ShippingPickerViewModel();

			var cart = GetCart();
			var shippingOption = GetShippingOptions().FirstOrDefault();
			var shippingMethods = GetShippingMethods(shippingOption, cart);
			var shippingMethodPrices = GetShippingMethodPrices(shippingMethods, cart);

			foreach (var availableShippingMethod in shippingMethods)
			{
				var shippingMethodPrice = shippingMethodPrices.FirstOrDefault(x => x.MethodId == availableShippingMethod.ExternalId);
				var formattedprice = string.Format("{0} {1}", shippingMethodPrice.Amount, shippingMethodPrice.CurrencyCode);

				shipmentPickerViewModel.AvailableShippingMethods.Add(new SelectListItem()
				{
					Selected = shipmentPickerViewModel.SelectedShippingMethodId.ToString() == availableShippingMethod.ShippingOptionId,
					Text = String.Format(" {0} ({1})", availableShippingMethod.Name, formattedprice),
					Value = availableShippingMethod.ExternalId
				});
			}

			return View("/Views/ShippingPicker/Rendering.cshtml", shipmentPickerViewModel);
		}

		[HttpPost]
		public ActionResult CreateShipment(ShippingPickerViewModel createShipmentViewModel)
		{
			_transactionLibraryInternal.CreateShipment(createShipmentViewModel.SelectedShippingMethodId, Constants.DefaultShipmentAddressName, true);
			_transactionLibraryInternal.ExecuteBasketPipeline();

			return Redirect("/payment");
		}

		private ReadOnlyCollection<ShippingOption> GetShippingOptions()
		{
			var shippingService = new ShippingServiceProvider();
			var request = new GetShippingOptionsRequest();
 
			return shippingService.GetShippingOptions(request).ShippingOptions;
		}

		private Cart GetCart()
		{
			var cartServiceProvider = new CartServiceProvider();

			var contactFactory = new ContactFactory();
			string userId = contactFactory.GetContact();

			var createCartRequest = new CreateOrResumeCartRequest(Context.GetSiteName(), userId);

			return cartServiceProvider.CreateOrResumeCart(createCartRequest).Cart;
		}

		private ReadOnlyCollection<ShippingMethod> GetShippingMethods(ShippingOption shippingOption, Cart cart)
		{
			var shippingService = new ShippingServiceProvider();
			var party = cart.Parties.FirstOrDefault(x => x.PartyId == cart.BuyerCustomerParty.PartyID);

			var request = new GetShippingMethodsRequest(shippingOption, party);

			return shippingService.GetShippingMethods(request).ShippingMethods;
		}

		private ReadOnlyCollection<ShippingPrice> GetShippingMethodPrices(ReadOnlyCollection<ShippingMethod> shippingMethods, Cart cart)
		{
			var shippingLookupList = new List<ShippingLookup>();

			foreach (var shippingMethod in shippingMethods)
			{
				shippingLookupList.Add(new ShippingLookup()
				{
					MethodId = shippingMethod.ExternalId
				});
			}

			var provider = (ShippingServiceProvider)Factory.CreateObject("shippingServiceProvider", true);
			var request = new GetPricesForShipmentsRequest(Context.GetSiteName(), shippingLookupList, cart);

			return provider.GetPricesForShipments(request).ShippingPrices;
		}
	}
}