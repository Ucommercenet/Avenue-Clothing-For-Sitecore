using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UCommerce.EntitiesV2;
using UCommerce.Transactions;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Transaction.ViewModels;
using Sitecore;
using Sitecore.Analytics;
using Sitecore.Commerce.Contacts;
using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Services.Carts;
using Constants = UCommerce.Constants;

namespace AvenueClothing.Project.Transaction.Controllers
{
    public class CommerceConnectAddressController : BaseController
    {
        private readonly TransactionLibraryInternal _transactionLibraryInternal;
		private readonly IRepository<Country> _countries;

		public CommerceConnectAddressController(TransactionLibraryInternal transactionLibraryInternal, IRepository<Country> countries)
        {
            _transactionLibraryInternal = transactionLibraryInternal;
            _countries = countries;
        }
        public ActionResult Rendering()
        {
            var viewModel = new AddressRenderingViewModel();

            var shippingInformation = _transactionLibraryInternal.GetBasket().PurchaseOrder.GetShippingAddress(Constants.DefaultShipmentAddressName) ?? new OrderAddress();
            var billingInformation = _transactionLibraryInternal.GetBasket().PurchaseOrder.BillingAddress ?? new OrderAddress();

            viewModel.BillingAddress.FirstName = billingInformation.FirstName;
            viewModel.BillingAddress.LastName = billingInformation.LastName;
            viewModel.BillingAddress.EmailAddress = billingInformation.EmailAddress;
            viewModel.BillingAddress.PhoneNumber = billingInformation.PhoneNumber;
            viewModel.BillingAddress.MobilePhoneNumber = billingInformation.MobilePhoneNumber;
            viewModel.BillingAddress.Line1 = billingInformation.Line1;
            viewModel.BillingAddress.Line2 = billingInformation.Line2;
            viewModel.BillingAddress.PostalCode = billingInformation.PostalCode;
            viewModel.BillingAddress.City = billingInformation.City;
            viewModel.BillingAddress.State = billingInformation.State;
            viewModel.BillingAddress.Attention = billingInformation.Attention;
            viewModel.BillingAddress.CompanyName = billingInformation.CompanyName;
            viewModel.BillingAddress.CountryId = billingInformation.Country != null ? billingInformation.Country.CountryId : -1;

            viewModel.ShippingAddress.FirstName = shippingInformation.FirstName;
            viewModel.ShippingAddress.LastName = shippingInformation.LastName;
            viewModel.ShippingAddress.EmailAddress = shippingInformation.EmailAddress;
            viewModel.ShippingAddress.PhoneNumber = shippingInformation.PhoneNumber;
            viewModel.ShippingAddress.MobilePhoneNumber = shippingInformation.MobilePhoneNumber;
            viewModel.ShippingAddress.Line1 = shippingInformation.Line1;
            viewModel.ShippingAddress.Line2 = shippingInformation.Line2;
            viewModel.ShippingAddress.PostalCode = shippingInformation.PostalCode;
            viewModel.ShippingAddress.City = shippingInformation.City;
            viewModel.ShippingAddress.State = shippingInformation.State;
            viewModel.ShippingAddress.Attention = shippingInformation.Attention;
            viewModel.ShippingAddress.CompanyName = shippingInformation.CompanyName;
            viewModel.ShippingAddress.CountryId = shippingInformation.Country != null ? shippingInformation.Country.CountryId : -1;

            viewModel.AvailableCountries = _countries.Select().Select(x => new SelectListItem() { Text = x.Name, Value = x.CountryId.ToString() }).ToList();

            viewModel.SaveAddressUrl = Url.Action("Save");

			return View("/Views/Address/Rendering.cshtml", viewModel);
        }

        [HttpPost]
        public ActionResult Save(AddressSaveViewModel addressRendering)
        {
            if (!addressRendering.IsShippingAddressDifferent)
            {
                this.ModelState.Remove("ShippingAddress.FirstName");
                this.ModelState.Remove("ShippingAddress.LastName");
                this.ModelState.Remove("ShippingAddress.EmailAddress");
                this.ModelState.Remove("ShippingAddress.Line1");
                this.ModelState.Remove("ShippingAddress.PostalCode");
                this.ModelState.Remove("ShippingAddress.City");
            }
            if (!ModelState.IsValid)
            {
                var dictionary = ModelState.ToDictionary(kvp => kvp.Key,
                 kvp => kvp.Value.Errors
                                 .Select(e => e.ErrorMessage).ToArray())
                                 .Where(m => m.Value.Any());

                return Json(new { modelStateErrors = dictionary });
            }

            if (addressRendering.IsShippingAddressDifferent)
            {
                EditBillingInformation(addressRendering.BillingAddress);
                EditShippingInformation(addressRendering.ShippingAddress);
            }

            else
            {
                EditBillingInformation(addressRendering.BillingAddress);
                EditShippingInformation(addressRendering.BillingAddress);
            }

            Tracker.Current.Session.CustomData["FirstName"] = addressRendering.BillingAddress.FirstName;
            
            _transactionLibraryInternal.ExecuteBasketPipeline();

            return Json(new {ShippingUrl = "/shipping"});

        }

        private void EditShippingInformation(AddressSaveViewModel.Address shippingAddress)
        {
			var cartServiceProvider = new CartServiceProvider();
	        var cart = GetCart();
			var party = new Party()
			{
				Address1 = shippingAddress.Line1,
				Address2 = shippingAddress.Line2,
				City = shippingAddress.City,
				Company = shippingAddress.CompanyName,
				Country = _countries.Select().First(x => x.CountryId == shippingAddress.CountryId).Name,
				Email = shippingAddress.EmailAddress,
				FirstName = shippingAddress.FirstName,
				LastName = shippingAddress.LastName,
				PartyId = Constants.DefaultShipmentAddressName,
				PhoneNumber = shippingAddress.PhoneNumber,
				State = shippingAddress.State,
				ZipPostalCode = shippingAddress.PostalCode
			};
			var partyList = new List<Party> { party };

			if (cart.Parties.Any(x => x.PartyId == party.PartyId))
			{
				var updatePartiesRequest = new UpdatePartiesRequest(cart, partyList);
				var updatePartiesResult = cartServiceProvider.UpdateParties(updatePartiesRequest);

				return;
			}

			var addPartiesRequest = new AddPartiesRequest(cart, partyList);
			var addPartiesResult = cartServiceProvider.AddParties(addPartiesRequest);
        }

        private void EditBillingInformation(AddressSaveViewModel.Address billingAddress)
        {
			var cartServiceProvider = new CartServiceProvider();
			var cart = GetCart();
			var party = new Party()
			{
				Address1 = billingAddress.Line1,
				Address2 = billingAddress.Line2,
				City = billingAddress.City,
				Company = billingAddress.CompanyName,
				Country = _countries.Select().First(x => x.CountryId == billingAddress.CountryId).Name,
				Email = billingAddress.EmailAddress,
				FirstName = billingAddress.FirstName,
				LastName = billingAddress.LastName,
				PartyId = "Billing",
				PhoneNumber = billingAddress.PhoneNumber,
				State = billingAddress.State,
				ZipPostalCode = billingAddress.PostalCode
			};
			var partyList = new List<Party> { party };

			if (cart.Parties.Any(x => x.PartyId == party.PartyId))
			{
				var updatePartiesRequest = new UpdatePartiesRequest(cart, partyList);
				var updatePartiesResult = cartServiceProvider.UpdateParties(updatePartiesRequest);

				return;
			}

			var addPartiesRequest = new AddPartiesRequest(cart, partyList);
			var addPartiesResult = cartServiceProvider.AddParties(addPartiesRequest);
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