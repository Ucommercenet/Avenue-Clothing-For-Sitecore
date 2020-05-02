
using System;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Transaction.ViewModels;
using Ucommerce;
using Ucommerce.EntitiesV2;

namespace AvenueClothing.Project.Transaction.Controllers
{
    public class ConfirmationEmailController : BaseController
    {
        private readonly IRepository<PurchaseOrder> _purchaseOrderRepository;

        public ConfirmationEmailController(IRepository<PurchaseOrder> purchaseOrderRepository)
        {
            _purchaseOrderRepository = purchaseOrderRepository;
        }

        public ActionResult Rendering()
        {
            var confirmationEmailViewModel = new ConfirmationEmailViewModel();
            var orderGuid = System.Web.HttpContext.Current.Request.QueryString["orderGuid"];

            if (!string.IsNullOrEmpty(orderGuid))
            {
                var purchaseOrder = _purchaseOrderRepository.SingleOrDefault(x => x.OrderGuid == new Guid(orderGuid));
                confirmationEmailViewModel = MapPurchaseOrderToViewModel(purchaseOrder, confirmationEmailViewModel);
            }

            return View(confirmationEmailViewModel);
        }

        private ConfirmationEmailViewModel MapPurchaseOrderToViewModel(PurchaseOrder purchaseOrder, ConfirmationEmailViewModel confirmationEmailViewModel)
        {
            confirmationEmailViewModel.BillingAddress = purchaseOrder.BillingAddress ?? new OrderAddress();
            confirmationEmailViewModel.ShipmentAddress = purchaseOrder.GetShippingAddress(Constants.DefaultShipmentAddressName) ?? new OrderAddress();

            foreach (var orderLine in purchaseOrder.OrderLines)
            {
                var orderLineModel = new ConfirmationEmailOrderLine
                {
                    ProductName = orderLine.ProductName,
                    Sku = orderLine.Sku,
                    VariantSku = orderLine.VariantSku,
                    Total = new Money(orderLine.Total.GetValueOrDefault(), orderLine.PurchaseOrder.BillingCurrency).ToString(),
                    Tax = new Money(orderLine.VAT, purchaseOrder.BillingCurrency).ToString(),
                    Price = new Money(orderLine.Price, purchaseOrder.BillingCurrency).ToString(),
                    PriceWithDiscount = new Money(orderLine.Price - orderLine.Discount, purchaseOrder.BillingCurrency).ToString(),
                    Quantity = orderLine.Quantity,
                    Discount = orderLine.Discount
                };

                confirmationEmailViewModel.OrderLines.Add(orderLineModel);
            }

            confirmationEmailViewModel.DiscountTotal = new Money(purchaseOrder.DiscountTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency).ToString();
            confirmationEmailViewModel.DiscountAmount = purchaseOrder.DiscountTotal.GetValueOrDefault();
            confirmationEmailViewModel.SubTotal = new Money(purchaseOrder.SubTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency).ToString();
            confirmationEmailViewModel.OrderTotal = new Money(purchaseOrder.OrderTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency).ToString();
            confirmationEmailViewModel.TaxTotal = new Money(purchaseOrder.TaxTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency).ToString();
            confirmationEmailViewModel.ShippingTotal = new Money(purchaseOrder.ShippingTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency).ToString();
            confirmationEmailViewModel.PaymentTotal = new Money(purchaseOrder.PaymentTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency).ToString();

            confirmationEmailViewModel.OrderNumber = purchaseOrder.OrderNumber;
            confirmationEmailViewModel.CustomerName = purchaseOrder.Customer.FirstName;

            var shipment = purchaseOrder.Shipments.FirstOrDefault();
            if (shipment != null)
            {
                confirmationEmailViewModel.ShipmentName = shipment.ShipmentName;
                confirmationEmailViewModel.ShipmentAmount = purchaseOrder.ShippingTotal.GetValueOrDefault();
            }

            var payment = purchaseOrder.Payments.FirstOrDefault();
            if (payment != null)
            {
                confirmationEmailViewModel.PaymentName = payment.PaymentMethodName;
                confirmationEmailViewModel.PaymentAmount = purchaseOrder.PaymentTotal.GetValueOrDefault();
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

            return confirmationEmailViewModel;
        }
    }
}