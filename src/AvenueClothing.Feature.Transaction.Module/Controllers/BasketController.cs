using AvenueClothing.Feature.Transaction.Module.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AvenueClothing.Feature.Transaction.Module.ViewModels.Basket;
using UCommerce;
using UCommerce.Api;
using UCommerce.EntitiesV2;

namespace AvenueClothing.Feature.Transaction.Module.Controllers
{
    public class BasketController : Controller
    {

        public ActionResult Rendering()
        {
            PurchaseOrder basket = UCommerce.Api.TransactionLibrary.GetBasket(false).PurchaseOrder;
            var basketModel = new PurchaseOrderViewModel();

            foreach (var orderLine in basket.OrderLines)
            {
                var orderLineViewModel = new OrderlineViewModel();

                orderLineViewModel.Quantity = orderLine.Quantity;
                orderLineViewModel.Product.Name = orderLine.ProductName;
                //orderLineViewModel.Product.SKU = orderLine.Sku;
                //orderLineViewModel.VariantSku = orderLine.VariantSku;
                orderLineViewModel.Total =
                    new Money(orderLine.Total.GetValueOrDefault(), basket.BillingCurrency).ToString();
                orderLineViewModel.Discount = orderLine.Discount;
                orderLineViewModel.Tax = new Money(orderLine.VAT, basket.BillingCurrency).ToString();
                orderLineViewModel.Price = new Money(orderLine.Price, basket.BillingCurrency).ToString();
                orderLineViewModel.Product.Url =
                    CatalogLibrary.GetNiceUrlForProduct(CatalogLibrary.GetProduct(orderLine.Sku));
                orderLineViewModel.OrderLineId = orderLine.OrderLineId;

                basketModel.OrderLines.Add(orderLineViewModel);
            }

            basketModel.OrderTotal = new Money(basket.OrderTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            basketModel.DiscountTotal =
                new Money(basket.DiscountTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            basketModel.TaxTotal = new Money(basket.TaxTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            basketModel.SubTotal = new Money(basket.SubTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();

            return View(basketModel);
        }

        [HttpPost]
        //public ActionResult Index(UpdatedOrderLinesViewModel model)
        //{
        //    foreach (var orderLine in model.UpdatedOrderLines)
        //    {
        //        var newQuantity = orderLine.Quantity;
        //        if (newQuantity <= 0)
        //        {
        //            newQuantity = 0;
        //        }
        //        TransactionLibrary.UpdateLineItem(orderLine.OrderLineId, newQuantity);
        //    }
        //    TransactionLibrary.ExecuteBasketPipeline();

        //    return Json(new { });
        //}}

        public ActionResult Index(PurchaseOrderViewModel model)
        {
            foreach (var orderLine in model.OrderLines)
            {
                var newQuantity = orderLine.Quantity;
                if (newQuantity <= 0)
                {
                    newQuantity = 0;
                }
                TransactionLibrary.UpdateLineItem(orderLine.OrderLineId, newQuantity);
            }
            TransactionLibrary.ExecuteBasketPipeline();

            return Json(new {});
        }
    }

}
