using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Transaction.ViewModels;
using Sitecore;
using Sitecore.Commerce.Contacts;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Services.Carts;
using UCommerce;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using Constants = UCommerce.Constants;

namespace AvenueClothing.Project.Transaction.Controllers
{
	public class CommerceConnectBasketPreviewController : BaseController
	{
		public ActionResult Rendering()
		{
			var basketPreviewViewModel = new BasketPreviewViewModel();

			//used to grap addresses
			var purchaseOrder = TransactionLibrary.GetBasket(false).PurchaseOrder;
			
			var cart = GetCart();
			
			basketPreviewViewModel = MapPurchaseOrderToViewModel(purchaseOrder, cart, basketPreviewViewModel);

			return View(basketPreviewViewModel);
		}

		[HttpPost]
		public ActionResult RequestPayment()
		{
			//TODO: Use federated payments, this is a temporary fix.
			var purchaseOrder = TransactionLibrary.GetBasket(false).PurchaseOrder;
			var payment  = purchaseOrder.Payments.First();
			payment.Amount = purchaseOrder.OrderTotal.Value;

			TransactionLibrary.RequestPayments();
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
					Total = new Money(cartLine.Total.Amount, currency).ToString(),
					Discount = new Money(cartLine.Adjustments.Sum(x => x.Amount), currency).Value,
					Price = new Money(cartLine.Product.Price.Amount, currency).ToString(),
					PriceWithDiscount = new Money((cartLine.Product.Price.Amount - cartLine.Adjustments.Sum(x => x.Amount)), currency).ToString(),
				};

				if (cartLine.GetPropertyValue("VariantSku") != null)
					orderLineViewModel.VariantSku = cartLine.GetPropertyValue("VariantSku").ToString();
				if (cartLine.Total.TaxTotal != null)
					orderLineViewModel.Tax = new Money(cartLine.Total.TaxTotal.Amount, currency).ToString();

				basketPreviewViewModel.OrderLines.Add(orderLineViewModel);
			}

			basketPreviewViewModel.DiscountTotal = new Money(cart.Adjustments.Sum(x => x.Amount), currency).ToString();
			basketPreviewViewModel.DiscountAmount = cart.Adjustments.Sum(x => x.Amount);
			basketPreviewViewModel.SubTotal = new Money((cart.Total.Amount - cart.Total.TaxTotal.Amount), currency).ToString();
			basketPreviewViewModel.OrderTotal = new Money(cart.Total.Amount, currency).ToString();
			if (cart.Total.TaxTotal != null)
				basketPreviewViewModel.TaxTotal = new Money(cart.Total.TaxTotal.Amount, currency).ToString();
			basketPreviewViewModel.ShippingTotal = new Money(purchaseOrder.ShippingTotal.GetValueOrDefault(), currency).ToString();
			basketPreviewViewModel.PaymentTotal = new Money(purchaseOrder.PaymentTotal.GetValueOrDefault(), currency).ToString();


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
	}
}