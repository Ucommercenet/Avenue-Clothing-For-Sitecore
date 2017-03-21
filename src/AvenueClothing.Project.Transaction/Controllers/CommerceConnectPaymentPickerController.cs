using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Transaction.Services;
using AvenueClothing.Project.Transaction.ViewModels;
using Sitecore;
using Sitecore.Commerce.Contacts;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Entities.Payments;
using Sitecore.Commerce.Services.Carts;
using Sitecore.Commerce.Services.Payments;
using UCommerce.Transactions;
using Constants = UCommerce.Constants;

namespace AvenueClothing.Project.Transaction.Controllers
{
    public class CommerceConnectPaymentPickerController : BaseController
    {
        private readonly ICurrencyFormatingService _currencyFormatingService;

        public CommerceConnectPaymentPickerController(ICurrencyFormatingService currencyFormatingService)
        {
            _currencyFormatingService = currencyFormatingService;
        }

     public ActionResult Rendering()
        {
            var paymentPickerViewModel = new PaymentPickerViewModel();

            paymentPickerViewModel.ControllerName = ControllerContext.RouteData.Values["controller"].ToString();
            paymentPickerViewModel.ActionName = "CreatePayment";

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

                CultureInfo cultureInfo = CultureInfo.GetCultureInfo(cart.CurrencyCode);
                var formattedFee = _currencyFormatingService.GetFormattedCurrencyString(fee, cultureInfo);
                option.Text = String.Format(" {0} ({1} + {2}%)", paymentMethod.Name, formattedFee,
                    feePercent.ToString("0.00"));
                option.Value = paymentMethod.ExternalId;
                option.Selected = paymentMethod.ExternalId == paymentPickerViewModel.SelectedPaymentMethodId.ToString();

                paymentPickerViewModel.AvailablePaymentMethods.Add(option);
            }

            return View("/Views/PaymentPicker/Rendering.cshtml", paymentPickerViewModel);
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