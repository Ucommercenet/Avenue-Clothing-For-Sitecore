using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Transaction.Services;
using AvenueClothing.Project.Transaction.ViewModels;
using UCommerce;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.Transactions;

namespace AvenueClothing.Project.Transaction.Controllers
{
	public class BasketController : BaseController
	{
	    private readonly TransactionLibraryInternal _transactionLibraryInternal;
	    private readonly IMiniBasketService _miniBasketService;
	    public BasketController(TransactionLibraryInternal transactionLibraryInternal, IMiniBasketService miniBasketService)
	    {
	        _transactionLibraryInternal = transactionLibraryInternal;
	        _miniBasketService = miniBasketService;
	    }

        public ActionResult Rendering()
        {
            PurchaseOrder basket = _transactionLibraryInternal.GetBasket(false).PurchaseOrder;
            var basketModel = new BasketRenderingViewModel();

            foreach (var orderLine in basket.OrderLines)
            {
                var orderLineViewModel = new BasketRenderingViewModel.OrderlineViewModel();

                orderLineViewModel.Quantity = orderLine.Quantity;
                orderLineViewModel.ProductName = orderLine.ProductName;
                orderLineViewModel.Sku = orderLine.Sku;
                orderLineViewModel.VariantSku = orderLine.VariantSku;
                orderLineViewModel.Total = new Money(orderLine.Total.GetValueOrDefault(), basket.BillingCurrency).ToString();
                orderLineViewModel.Discount = orderLine.Discount;
                orderLineViewModel.Tax = new Money(orderLine.VAT, basket.BillingCurrency).ToString();
                orderLineViewModel.Price = new Money(orderLine.Price, basket.BillingCurrency).ToString();
                orderLineViewModel.ProductUrl = CatalogLibrary.GetNiceUrlForProduct(CatalogLibrary.GetProduct(orderLine.Sku));
                orderLineViewModel.PriceWithDiscount = new Money(orderLine.Price - orderLine.Discount, basket.BillingCurrency).ToString();
                orderLineViewModel.OrderLineId = orderLine.OrderLineId;

                basketModel.OrderLines.Add(orderLineViewModel);
            }

            basketModel.OrderTotal = new Money(basket.OrderTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            basketModel.DiscountTotal = new Money(basket.DiscountTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            basketModel.TaxTotal = new Money(basket.TaxTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            basketModel.SubTotal = new Money(basket.SubTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();

            basketModel.RefreshUrl = Url.Action("UpdateBasket");

            return View(basketModel);
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

	        _transactionLibraryInternal.UpdateLineItemByOrderLineId(updateOrderline.OrderLineId, newQuantity);
	        }

	        _transactionLibraryInternal.ExecuteBasketPipeline();

	        var basket = _transactionLibraryInternal.GetBasket(false).PurchaseOrder;

            BasketUpdateBasketViewModel updatedBasket = new BasketUpdateBasketViewModel();

            foreach (var orderLine in basket.OrderLines)
            {
                var orderLineViewModel = new BasketUpdateOrderline();
                orderLineViewModel.OrderlineId = orderLine.OrderLineId;
                orderLineViewModel.Quantity = orderLine.Quantity;
                orderLineViewModel.Total = new Money(orderLine.Total.GetValueOrDefault(), basket.BillingCurrency).ToString();
                orderLineViewModel.Discount = orderLine.Discount;
                orderLineViewModel.Tax = new Money(orderLine.VAT, basket.BillingCurrency).ToString();
                orderLineViewModel.Price = new Money(orderLine.Price, basket.BillingCurrency).ToString();
                orderLineViewModel.PriceWithDiscount = new Money(orderLine.Price - orderLine.Discount, basket.BillingCurrency).ToString();

                updatedBasket.Orderlines.Add(orderLineViewModel);
            }

            string orderTotal = new Money(basket.OrderTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            string discountTotal = new Money(basket.DiscountTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            string taxTotal = new Money(basket.TaxTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            string subTotal = new Money(basket.SubTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();

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
