using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Transaction.Services;
using AvenueClothing.Project.Transaction.ViewModels;
using Ucommerce;
using Ucommerce.Api;
using Ucommerce.EntitiesV2;
using Ucommerce.Search.Slugs;

namespace AvenueClothing.Project.Transaction.Controllers
{
    public class BasketController : BaseController
    {
        private readonly ITransactionLibrary _transactionLibrary;
        private readonly IMiniBasketService _miniBasketService;
        private readonly IUrlService _urlService;
        private readonly ICatalogContext _catalogContext;
        private readonly ICatalogLibrary _catalogLibrary;

        public BasketController(ITransactionLibrary transactionLibrary, IMiniBasketService miniBasketService, IUrlService urlService, ICatalogContext catalogContext, ICatalogLibrary catalogLibrary)
        {
            _transactionLibrary = transactionLibrary;
            _miniBasketService = miniBasketService;
            _urlService = urlService;
            _catalogContext = catalogContext;
            _catalogLibrary = catalogLibrary;
        }

        public ActionResult Rendering()
        {
			var basketModel = new BasketRenderingViewModel();

	        if (!_transactionLibrary.HasBasket())
	        {
				return View(basketModel);
	        }

            PurchaseOrder basket = _transactionLibrary.GetBasket();

            foreach (var orderLine in basket.OrderLines)
            {
                var orderLineViewModel = new BasketRenderingViewModel.OrderlineViewModel();

                orderLineViewModel.Quantity = orderLine.Quantity;
                orderLineViewModel.ProductName = orderLine.ProductName;
                orderLineViewModel.Sku = orderLine.Sku;
                orderLineViewModel.VariantSku = orderLine.VariantSku;
                orderLineViewModel.Total = new Money(orderLine.Total.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString();
                orderLineViewModel.Discount = orderLine.Discount;
                orderLineViewModel.Tax = new Money(orderLine.VAT, basket.BillingCurrency.ISOCode).ToString();
                orderLineViewModel.Price = new Money(orderLine.Price, basket.BillingCurrency.ISOCode).ToString();
                orderLineViewModel.ProductUrl = _urlService.GetUrl(_catalogContext.CurrentCatalog,
                    _catalogLibrary.GetProduct(orderLine.Sku));
                orderLineViewModel.PriceWithDiscount = new Money(orderLine.Price - orderLine.UnitDiscount.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString();
                orderLineViewModel.OrderLineId = orderLine.OrderLineId;

                basketModel.OrderLines.Add(orderLineViewModel);
            }

            basketModel.OrderTotal = new Money(basket.OrderTotal.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString();
            basketModel.DiscountTotal = new Money(basket.DiscountTotal.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString();
            basketModel.TaxTotal = new Money(basket.TaxTotal.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString();
            basketModel.SubTotal = new Money(basket.SubTotal.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString();

            basketModel.RefreshUrl = Url.Action("UpdateBasket");
            basketModel.RemoveOrderlineUrl = Url.Action("RemoveOrderline");

            return View(basketModel);
        }

        [HttpPost]
        public ActionResult RemoveOrderline(int orderlineId)
        {
            _transactionLibrary.UpdateLineItemByOrderLineId(orderlineId, 0);

            return Json(new
            {
                orderlineId = orderlineId
            });
        }


        [HttpPost]
        public ActionResult UpdateBasket(BasketUpdateBasket model)
        {
            foreach (var updateOrderline in model.RefreshBasket)
            {
                var newQuantity = updateOrderline.OrderLineQty;
                if (newQuantity <= 0)
                {
                    newQuantity = 0;
                }

                _transactionLibrary.UpdateLineItemByOrderLineId(updateOrderline.OrderLineId, newQuantity);
            }

            _transactionLibrary.ExecuteBasketPipeline();

            var basket = _transactionLibrary.GetBasket();

            BasketUpdateBasketViewModel updatedBasket = new BasketUpdateBasketViewModel();

            foreach (var orderLine in basket.OrderLines)
            {
                var orderLineViewModel = new BasketUpdateOrderline();
                orderLineViewModel.OrderlineId = orderLine.OrderLineId;
                orderLineViewModel.Quantity = orderLine.Quantity;
                orderLineViewModel.Total = new Money(orderLine.Total.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString();
                orderLineViewModel.Discount = orderLine.Discount;
                orderLineViewModel.Tax = new Money(orderLine.VAT, basket.BillingCurrency.ISOCode).ToString();
                orderLineViewModel.Price = new Money(orderLine.Price, basket.BillingCurrency.ISOCode).ToString();
                orderLineViewModel.PriceWithDiscount = new Money(orderLine.Price - orderLine.Discount, basket.BillingCurrency.ISOCode).ToString();

                updatedBasket.Orderlines.Add(orderLineViewModel);
            }

            string orderTotal = new Money(basket.OrderTotal.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString();
            string discountTotal = new Money(basket.DiscountTotal.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString();
            string taxTotal = new Money(basket.TaxTotal.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString();
            string subTotal = new Money(basket.SubTotal.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString();

            updatedBasket.OrderTotal = orderTotal;
            updatedBasket.DiscountTotal = discountTotal;
            updatedBasket.TaxTotal = taxTotal;
            updatedBasket.SubTotal = subTotal;


            return Json(new
            {
                MiniBasketRefresh = _miniBasketService.Refresh(),
                OrderTotal = orderTotal,
                DiscountTotal = discountTotal,
                TaxTotal = taxTotal,
                SubTotal = subTotal,
                OrderLines = updatedBasket.Orderlines
            });
        }



    }

}
