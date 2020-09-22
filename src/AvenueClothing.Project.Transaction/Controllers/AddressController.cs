﻿using System;
using System.Linq;
using System.Web.Mvc;
using Ucommerce.EntitiesV2;
using Ucommerce.Transactions;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Transaction.ViewModels;
using Sitecore.Analytics;
using Ucommerce.Api;
using Constants = Ucommerce.Constants;

namespace AvenueClothing.Project.Transaction.Controllers
{
    public class AddressController : BaseController
    {
        private readonly ITransactionLibrary _transactionLibrary;
        private readonly IQueryable<Country> _countries;

        public AddressController(ITransactionLibrary transactionLibrary, IQueryable<Country> countries)
        {
            _transactionLibrary = transactionLibrary;
            _countries = countries;
        }
        public ActionResult Rendering()
        {
            var viewModel = new AddressRenderingViewModel();

            var shippingInformation = _transactionLibrary.GetBasket().GetShippingAddress(Constants.DefaultShipmentAddressName) ?? new OrderAddress();
            var billingInformation = _transactionLibrary.GetBasket().BillingAddress ?? new OrderAddress();

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

            viewModel.AvailableCountries = _countries.ToList().Select(x => new SelectListItem() { Text = x.Name, Value = x.CountryId.ToString() }).ToList();

            viewModel.SaveAddressUrl = Url.Action("Save");

            return View(viewModel);
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

            if(Tracker.Current != null)
                Tracker.Current.Session.CustomData["FirstName"] = addressRendering.BillingAddress.FirstName;

            _transactionLibrary.ExecuteBasketPipeline();

            return Json(new {ShippingUrl = "/shipping"});

        }

        private void EditShippingInformation(AddressSaveViewModel.Address shippingAddress)
        {
            _transactionLibrary.EditShipmentInformation(
                Constants.DefaultShipmentAddressName,
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

        private void EditBillingInformation(AddressSaveViewModel.Address billingAddress)
        {
            _transactionLibrary.EditBillingInformation(
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