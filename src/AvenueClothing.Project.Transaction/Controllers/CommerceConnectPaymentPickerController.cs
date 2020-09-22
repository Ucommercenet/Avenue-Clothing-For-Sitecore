﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Transaction.ViewModels;
using Sitecore;
using Sitecore.Commerce.Contacts;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Entities.Payments;
using Sitecore.Commerce.Entities.Shipping;
using Sitecore.Commerce.Services.Carts;
using Sitecore.Commerce.Services.Payments;
using Sitecore.Commerce.Services.Shipping;
using Sitecore.Configuration;
using Ucommerce;
using Ucommerce.Api;
using Ucommerce.Transactions;
using Constants = Ucommerce.Constants;

namespace AvenueClothing.Project.Transaction.Controllers
{
    public class CommerceConnectPaymentPickerController : BaseController
    {
        private readonly ITransactionLibrary _transactionLibrary;

        public CommerceConnectPaymentPickerController(ITransactionLibrary transactionLibrary)
        {
            _transactionLibrary = transactionLibrary;
        }

        public ActionResult Rendering()
        {
            var paymentPickerViewModel = new PaymentPickerViewModel();


            var cart = GetCart();
            var paymentOption = GetPaymentOptions(cart.ShopName).FirstOrDefault();
            var paymentMethods = GetPaymentMethods(paymentOption, cart);
            var paymentMethodPrices = GetPaymentMethodPrices(paymentMethods, cart);


            var payment = cart.Payment.FirstOrDefault();

            if (payment != null)
            {
                int paymentMethodId;
                if (int.TryParse(payment.PaymentMethodID, out paymentMethodId))
                {
                    paymentPickerViewModel.SelectedPaymentMethodId = paymentMethodId;
                }
            }

            foreach (var paymentMethod in paymentMethods)
            {
                var option = new SelectListItem();
                var paymentPrice = paymentMethodPrices.First(x => x.MethodId == paymentMethod.ExternalId);
                decimal feePercent = (decimal)paymentPrice.GetPropertyValue("FeePercent");
                var fee = paymentPrice.Amount;
                var formattedFee = string.Format("{0} {1}", fee, paymentPrice.CurrencyCode);

                option.Text = String.Format(" {0} ({1} + {2}%)", paymentMethod.Name, formattedFee,
                    feePercent.ToString("0.00"));
                option.Value = paymentMethod.ExternalId;
                option.Selected = paymentMethod.ExternalId == paymentPickerViewModel.SelectedPaymentMethodId.ToString();

                paymentPickerViewModel.AvailablePaymentMethods.Add(option);
            }

            return View(paymentPickerViewModel);
        }

        [HttpPost]
        public ActionResult CreatePayment(PaymentPickerViewModel createPaymentViewModel)
        {
            var cartService = new CartServiceProvider();
            var cart = GetCart();

			// First remove all the existing payment info's associated with the cart.
	        var removeRequest = new RemovePaymentInfoRequest(cart, cart.Payment.ToList());
	        cartService.RemovePaymentInfo(removeRequest);

            var paymentList = new List<PaymentInfo>
            {
                new PaymentInfo()
                {
                    PaymentMethodID = createPaymentViewModel.SelectedPaymentMethodId.ToString(),
                }
            };

            var addRequest = new AddPaymentInfoRequest(cart, paymentList);
            cartService.AddPaymentInfo(addRequest);

            return Redirect("/preview");
        }

        private Cart GetCart()
        {
            var cartServiceProvider = new CartServiceProvider();

            var contactFactory = new ContactFactory();
            string userId = contactFactory.GetContact();

            var createCartRequest = new CreateOrResumeCartRequest(Context.GetSiteName(), userId);

            return cartServiceProvider.CreateOrResumeCart(createCartRequest).Cart;
        }

        private ReadOnlyCollection<PaymentOption> GetPaymentOptions(string shopName)
        {
            var paymentService = new PaymentServiceProvider();
            var request = new GetPaymentOptionsRequest(shopName);

            return paymentService.GetPaymentOptions(request).PaymentOptions;
        }

        private ReadOnlyCollection<PaymentMethod> GetPaymentMethods(PaymentOption paymentOption, Cart cart)
        {
            var paymentService = new PaymentServiceProvider();
            var request = new GetPaymentMethodsRequest(paymentOption, cart.ShopName);

            var shippingParty = cart.Parties.FirstOrDefault(x => (string)x.Properties["Name"] == Constants.DefaultShipmentAddressName);
            if (shippingParty != null)
            {
                request.Properties["Country"] = shippingParty.Country;
            }

            return paymentService.GetPaymentMethods(request).PaymentMethods;
        }

        private ReadOnlyCollection<PaymentPrice> GetPaymentMethodPrices(ReadOnlyCollection<PaymentMethod> paymentMethods, Cart cart)
        {
            var paymentLookupList = new List<PaymentLookup>();

            foreach (var paymentMethod in paymentMethods)
            {
                paymentLookupList.Add(new PaymentLookup()
                {
                    MethodId = paymentMethod.PaymentOptionId
                });
            }

            var paymentService = new PaymentServiceProvider();
            var request = new GetPricesForPaymentsRequest(cart.ShopName, paymentLookupList, cart);

            return paymentService.GetPricesForPayments(request).PaymentPrices;
        }
    }
}