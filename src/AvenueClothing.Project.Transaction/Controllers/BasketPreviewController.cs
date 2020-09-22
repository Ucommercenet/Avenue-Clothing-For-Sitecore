using System.Linq;
using System.Net;
using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Transaction.ViewModels;
using Ucommerce;
using Ucommerce.Api;
using Ucommerce.EntitiesV2;
using Ucommerce.Transactions;

namespace AvenueClothing.Project.Transaction.Controllers
{
	public class BasketPreviewController : BaseController
	{
		private readonly ITransactionLibrary _transactionLibrary;

		public BasketPreviewController(ITransactionLibrary transactionLibrary)
		{
			_transactionLibrary = transactionLibrary;
		}

		public ActionResult Rendering()
		{
			var purchaseOrder = _transactionLibrary.GetBasket();
			var basketPreviewViewModel = new BasketPreviewViewModel();

			basketPreviewViewModel = MapPurchaseOrderToViewModel(purchaseOrder, basketPreviewViewModel);

			return View(basketPreviewViewModel);
		}

		[HttpPost]
		public ActionResult RequestPayment()
		{
		    var payment = _transactionLibrary.GetBasket().Payments.First();
		    if (payment.PaymentMethod.PaymentMethodServiceName == null)
		    {
		        return Redirect("/confirmation");
            }

		    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string paymentUrl = _transactionLibrary.GetPaymentPageUrl(payment);
		    return Redirect(paymentUrl);
		}

		private BasketPreviewViewModel MapPurchaseOrderToViewModel(PurchaseOrder purchaseOrder, BasketPreviewViewModel basketPreviewViewModel)
		{

			basketPreviewViewModel.BillingAddress = purchaseOrder.BillingAddress ?? new OrderAddress();
			basketPreviewViewModel.ShipmentAddress = purchaseOrder.GetShippingAddress(Constants.DefaultShipmentAddressName) ?? new OrderAddress();

			foreach (var orderLine in purchaseOrder.OrderLines)
			{
				var orderLineModel = new PreviewOrderLine
				{
					ProductName = orderLine.ProductName,
					Sku = orderLine.Sku,
					VariantSku = orderLine.VariantSku,
					Total = new Money(orderLine.Total.GetValueOrDefault(), orderLine.PurchaseOrder.BillingCurrency.ISOCode).ToString(),
					Tax = new Money(orderLine.VAT, purchaseOrder.BillingCurrency.ISOCode).ToString(),
					Price = new Money(orderLine.Price, purchaseOrder.BillingCurrency.ISOCode).ToString(),
					PriceWithDiscount = new Money(orderLine.Price - orderLine.UnitDiscount.GetValueOrDefault(), purchaseOrder.BillingCurrency.ISOCode).ToString(),
					Quantity = orderLine.Quantity,
					Discount = orderLine.Discount
				};

				basketPreviewViewModel.OrderLines.Add(orderLineModel);
			}

			basketPreviewViewModel.DiscountTotal = new Money(purchaseOrder.DiscountTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency.ISOCode).ToString();
			basketPreviewViewModel.DiscountAmount = purchaseOrder.DiscountTotal.GetValueOrDefault();
			basketPreviewViewModel.SubTotal = new Money(purchaseOrder.SubTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency.ISOCode).ToString();
			basketPreviewViewModel.OrderTotal = new Money(purchaseOrder.OrderTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency.ISOCode).ToString();
			basketPreviewViewModel.TaxTotal = new Money(purchaseOrder.TaxTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency.ISOCode).ToString();
			basketPreviewViewModel.ShippingTotal = new Money(purchaseOrder.ShippingTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency.ISOCode).ToString();
			basketPreviewViewModel.PaymentTotal = new Money(purchaseOrder.PaymentTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency.ISOCode).ToString();


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
			if (purchaseOrder.DiscountTotal > 0)
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
	}
}