using System.Linq;
using System.Net;
using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Transaction.ViewModels;
using UCommerce;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.Transactions;

namespace AvenueClothing.Project.Transaction.Controllers
{
	public class BasketPreviewController : BaseController
	{
		private readonly TransactionLibraryInternal _transactionLibraryInternal;

		public BasketPreviewController(TransactionLibraryInternal transactionLibraryInternal)
		{
			_transactionLibraryInternal = transactionLibraryInternal;
		}

		public ActionResult Rendering()
		{
			var purchaseOrder = _transactionLibraryInternal.GetBasket(false).PurchaseOrder;
			var basketPreviewViewModel = new BasketPreviewViewModel();

			basketPreviewViewModel = MapPurchaseOrderToViewModel(purchaseOrder, basketPreviewViewModel);

			return View(basketPreviewViewModel);
		}

		[HttpPost]
		public ActionResult RequestPayment()
		{
		    var payment = _transactionLibraryInternal.GetBasket().PurchaseOrder.Payments.First();
		    if (payment.PaymentMethod.PaymentMethodServiceName == null)
		    {
		        return Redirect("/confirmation");
            }

		    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string paymentUrl = _transactionLibraryInternal.GetPaymentPageUrl(payment);
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
					Total = new Money(orderLine.Total.GetValueOrDefault(), orderLine.PurchaseOrder.BillingCurrency).ToString(),
					Tax = new Money(orderLine.VAT, purchaseOrder.BillingCurrency).ToString(),
					Price = new Money(orderLine.Price, purchaseOrder.BillingCurrency).ToString(),
					PriceWithDiscount = new Money(orderLine.Price - orderLine.UnitDiscount.GetValueOrDefault(), purchaseOrder.BillingCurrency).ToString(),
					Quantity = orderLine.Quantity,
					Discount = orderLine.Discount
				};

				basketPreviewViewModel.OrderLines.Add(orderLineModel);
			}

			basketPreviewViewModel.DiscountTotal = new Money(purchaseOrder.DiscountTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency).ToString();
			basketPreviewViewModel.DiscountAmount = purchaseOrder.DiscountTotal.GetValueOrDefault();
			basketPreviewViewModel.SubTotal = new Money(purchaseOrder.SubTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency).ToString();
			basketPreviewViewModel.OrderTotal = new Money(purchaseOrder.OrderTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency).ToString();
			basketPreviewViewModel.TaxTotal = new Money(purchaseOrder.TaxTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency).ToString();
			basketPreviewViewModel.ShippingTotal = new Money(purchaseOrder.ShippingTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency).ToString();
			basketPreviewViewModel.PaymentTotal = new Money(purchaseOrder.PaymentTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency).ToString();


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