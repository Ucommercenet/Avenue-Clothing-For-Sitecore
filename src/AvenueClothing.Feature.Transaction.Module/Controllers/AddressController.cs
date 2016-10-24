using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Feature.Transaction.Module.ViewModels;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;
using Sitecore.Mvc.Presentation;
using UCommerce;
using UCommerce.Api;
using UCommerce.EntitiesV2;

namespace AvenueClothing.Feature.Transaction.Module.Controllers
{
	public class AddressController : Controller
	{
		public ActionResult Rendering()
		{
			var viewModel = new AddressRenderingViewModel();

			var shippingInformation = TransactionLibrary.GetShippingInformation();
			var billingInformation = TransactionLibrary.GetBillingInformation();

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

			viewModel.AvailableCountries = Country.All().ToList().Select(x => new SelectListItem() { Text = x.Name, Value = x.CountryId.ToString() }).ToList();

			return View(viewModel);
		}

		[HttpPost]
		public ActionResult Save(AddressRenderingViewModel addressRendering)
		{
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

			TransactionLibrary.ExecuteBasketPipeline();

			return Redirect("/basket/shipping");
		}

		private void EditShippingInformation(AddressViewModel shippingAddress)
		{
			TransactionLibrary.EditShippingInformation(
		  shippingAddress.FirstName,
		  shippingAddress.LastName,
		  shippingAddress.EmailAddress,
		  shippingAddress.PhoneNumber,
		  shippingAddress.MobilePhoneNumber,
		  shippingAddress.CompanyName,
		  shippingAddress.Line1,
		  shippingAddress.Line2,
		  shippingAddress.PostalCode,
		  shippingAddress.City,
		  shippingAddress.State,
		  shippingAddress.Attention,
		  shippingAddress.CountryId);
		}

		private void EditBillingInformation(AddressViewModel billingAddress)
		{
			TransactionLibrary.EditBillingInformation(
			   billingAddress.FirstName,
			   billingAddress.LastName,
			   billingAddress.EmailAddress,
			   billingAddress.PhoneNumber,
			   billingAddress.MobilePhoneNumber,
			   billingAddress.CompanyName,
			   billingAddress.Line1,
			   billingAddress.Line2,
			   billingAddress.PostalCode,
			   billingAddress.City,
			   billingAddress.State,
			   billingAddress.Attention,
			   billingAddress.CountryId);
		}
	}
}