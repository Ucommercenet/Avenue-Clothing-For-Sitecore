using System.Linq;
using System.Web;
using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Transaction.ViewModels;
using Sitecore;
using Sitecore.Commerce.Contacts;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Services.Carts;
using Sitecore.Commerce.Services.Orders;
using Sitecore.Commerce.Services.Payments;
using Ucommerce;
using Ucommerce.Api;
using Ucommerce.EntitiesV2;
using Ucommerce.Infrastructure;
using Ucommerce.Transactions.Payments;
using Constants = Ucommerce.Constants;

namespace AvenueClothing.Project.Transaction.Controllers
{
	public class CommerceConnectBasketPreviewController : BaseController
	{
		private readonly ITransactionLibrary _transactionLibrary;

		public CommerceConnectBasketPreviewController(ITransactionLibrary transactionLibrary)
		{
			_transactionLibrary = transactionLibrary;
		}

		public ActionResult Rendering()
		{
			var basketPreviewViewModel = new BasketPreviewViewModel();

			//used to grap addresses
			var purchaseOrder = _transactionLibrary.GetBasket();

			var cart = GetCart();

			basketPreviewViewModel = MapPurchaseOrderToViewModel(purchaseOrder, cart, basketPreviewViewModel);

			return View(basketPreviewViewModel);
		}

		[HttpPost]
		public ActionResult RequestPayment()
		{
			// If you are OK with calling a uCommerce API at this point, you can simply call:

			// --- BEGIN uCommerce API.

			// TransactionLibrary.RequestPayments();
			// return Redirect("/confirmation"); // This line is only required when using the DefaultPaymentMethod for the demo store.

			// --- END uCommerce API.

			// If you want to use the FederatedPayment parts of Commerce Connect. This is what you need to do, to make it work
			var cart = GetCart();

			// For this demo store, it is assumed, that there is only one payment info associated with the order.

			// 1. You need to get the payment service URL for the payment
			var paymentService = new PaymentServiceProvider();
			var baseUrl = HttpUtility.UrlDecode(paymentService.GetPaymentServiceUrl(new GetPaymentServiceUrlRequest()).Url) ?? string.Empty;

			// 2. You then need to add the payment method id and the payment id to the Url returned from the service.
			var paymentInfo = cart.Payment.First();
			var completeUrl = string.Format(baseUrl, paymentInfo.PaymentMethodID, paymentInfo.ExternalId);

			// 3. In an IFrame set the url to the url from step 2.
			// Because this is a demo store, there is no actual payment gateway involved
			// Therefore at this point we need to manually set the status of the payment to Authorized.

			// ONLY CALLED FOR DEMO PURPOSES
			SetPaymentStatusToAuthorized(paymentInfo.ExternalId);
			// You should redirect the IFrame to the "completeUrl".

			// If you have configured your payment method to run the Checkout pipeline,
			// upon completion, then you do not need to do anything else.
			// The uCommerce framework takes care of the rest.
			// It checks that the transaction was authorized, and it redirects the customer to the accept url.
			// And it calls the Commerce Connect. SubmitVisitorOrder pipeline.
			// So you are done!

			// If you insist on doing the payment in the full Commerce Connect experience,
			// you should configure the uCommerce payment method to not run any pipeline on completion.

			// And in that case, you need to check that the payment was OK, and do the SubmitVisitorOrder calls yourself.
			// These steps are described below.

			// 4. When you believe that the transaction has been completed on the hosted page, you need to check if the payment is OK.
			// In the uCommerce implementation of the Commerce Connect API, this is done by passing the payment id as the access code.
			var actionResult = paymentService.GetPaymentServiceActionResult(new GetPaymentServiceActionResultRequest() { PaymentAcceptResultAccessCode = paymentInfo.ExternalId });
			var paymentWasOk = actionResult.AuthorizationResult == "OK";

			// 5. If the payment is OK, then you can Submit the Order. This corresponds to running the "Checkout" pipeline in uCommerce.
			if (paymentWasOk)
			{
				var orderService = new OrderServiceProvider();
				var request = new SubmitVisitorOrderRequest(cart);
				orderService.SubmitVisitorOrder(request);
			}

			return Redirect("/confirmation");
		}

		private BasketPreviewViewModel MapPurchaseOrderToViewModel(PurchaseOrder purchaseOrder, Cart cart, BasketPreviewViewModel basketPreviewViewModel)
		{
			basketPreviewViewModel.BillingAddress = purchaseOrder.BillingAddress ?? new OrderAddress();
			basketPreviewViewModel.ShipmentAddress = purchaseOrder.GetShippingAddress(Constants.DefaultShipmentAddressName) ?? new OrderAddress();

			var currency = new Currency()
			{
				ISOCode = cart.CurrencyCode
			};

			foreach (var cartLine in cart.Lines)
			{
				var orderLineViewModel = new PreviewOrderLine
				{
					Quantity = (int)cartLine.Quantity,
					ProductName = cartLine.Product.ProductName,
					Sku = cartLine.Product.ProductId,
					Total = new Money(cartLine.Total.Amount, currency.ISOCode).ToString(),
					Discount = new Money(cartLine.Adjustments.Sum(x => x.Amount), currency.ISOCode).Value,
					Price = new Money(cartLine.Product.Price.Amount, currency.ISOCode).ToString(),
					PriceWithDiscount = new Money((cartLine.Product.Price.Amount - cartLine.Adjustments.Sum(x => x.Amount)), currency.ISOCode).ToString(),
				};

				if (cartLine.GetPropertyValue("VariantSku") != null)
					orderLineViewModel.VariantSku = cartLine.GetPropertyValue("VariantSku").ToString();
				if (cartLine.Total.TaxTotal != null)
					orderLineViewModel.Tax = new Money(cartLine.Total.TaxTotal.Amount, currency.ISOCode).ToString();

				basketPreviewViewModel.OrderLines.Add(orderLineViewModel);
			}

			basketPreviewViewModel.DiscountTotal = new Money(cart.Adjustments.Sum(x => x.Amount), currency.ISOCode).ToString();
			basketPreviewViewModel.DiscountAmount = cart.Adjustments.Sum(x => x.Amount);
			basketPreviewViewModel.SubTotal = new Money((cart.Total.Amount - cart.Total.TaxTotal.Amount), currency.ISOCode).ToString();
			basketPreviewViewModel.OrderTotal = new Money(cart.Total.Amount, currency.ISOCode).ToString();
			if (cart.Total.TaxTotal != null)
				basketPreviewViewModel.TaxTotal = new Money(cart.Total.TaxTotal.Amount, currency.ISOCode).ToString();
			basketPreviewViewModel.ShippingTotal = new Money(purchaseOrder.ShippingTotal.GetValueOrDefault(), currency.ISOCode).ToString();
			basketPreviewViewModel.PaymentTotal = new Money(purchaseOrder.PaymentTotal.GetValueOrDefault(), currency.ISOCode).ToString();


			var shipment = purchaseOrder.Shipments.FirstOrDefault();
			if (shipment != null)
			{
				basketPreviewViewModel.ShipmentName = shipment.ShipmentName;
				basketPreviewViewModel.ShipmentAmount = purchaseOrder.ShippingTotal.GetValueOrDefault();
			}

			var payment = purchaseOrder.Payments.FirstOrDefault();
			if (payment != null)
			{
				basketPreviewViewModel.PaymentName = payment.PaymentMethodName;
				basketPreviewViewModel.PaymentAmount = purchaseOrder.PaymentTotal.GetValueOrDefault();
			}

			ViewBag.RowSpan = 4;
			if (cart.Adjustments.Sum(x => x.Amount) > 0)
			{
				ViewBag.RowSpan++;
			}
			if (purchaseOrder.ShippingTotal > 0)
			{
				ViewBag.RowSpan++;
			}
			if (purchaseOrder.PaymentTotal > 0)
			{
				ViewBag.RowSpan++;
			}

			return basketPreviewViewModel;
		}

		private Cart GetCart()
		{
			var cartServiceProvider = new CartServiceProvider();

			var contactFactory = new ContactFactory();
			string userId = contactFactory.GetContact();

			var createCartRequest = new CreateOrResumeCartRequest(Context.GetSiteName(), userId);

			return cartServiceProvider.CreateOrResumeCart(createCartRequest).Cart;
		}

		private void SetPaymentStatusToAuthorized(string paymentInfoExternalId)
		{
			var paymentRepository = ObjectFactory.Instance.Resolve<IRepository<Payment>>();
			var paymentStatusRepository = ObjectFactory.Instance.Resolve<IRepository<PaymentStatus>>();

			var payment = paymentRepository.Get(int.Parse(paymentInfoExternalId));
			var authorizedStatus = paymentStatusRepository.Get((int) PaymentStatusCode.Authorized);
			payment.PaymentStatus = authorizedStatus;
			payment.Save();
		}

	}
}