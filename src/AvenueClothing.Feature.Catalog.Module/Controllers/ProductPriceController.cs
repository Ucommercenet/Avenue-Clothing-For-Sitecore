using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AvenueClothing.Feature.Catalog.Module.ViewModels;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.Runtime;

namespace AvenueClothing.Feature.Catalog.Module.Controllers
{
    public class ProductPriceController:Controller
    {

        public ActionResult Rendering()
        {
            var currentProduct = SiteContext.Current.CatalogContext.CurrentProduct;
            var currentCategory = SiteContext.Current.CatalogContext.CurrentCategory;
            var currentCatalog = SiteContext.Current.CatalogContext.CurrentCatalog;

            ProductPriceRenderingViewModel productPriceRenderingViewModelModel = new ProductPriceRenderingViewModel()
            {
                CategoryGuid = currentCategory.Guid,
                CalculatePriceUrl = Url.Action("CalculatePrice"),
                CatalogGuid = currentCatalog.Id,
                SKU = currentProduct.Sku
            };
            
            return View(productPriceRenderingViewModelModel);
        }

        [HttpPost]
        public ActionResult CalculatePrice(PriceCalculationDetails priceCalculationDetails)
        {
            Product product = Product.FirstOrDefault(x=>x.Sku == priceCalculationDetails.ProductSku);
            var catalog = CatalogLibrary.GetCatalog(priceCalculationDetails.CatalogId);
            PriceCalculation priceCalc = CatalogLibrary.CalculatePrice(product, catalog);
            var yourPrice = priceCalc.YourPrice.Amount.ToString();
            var yourTax = priceCalc.YourTax.ToString();
            return Json(new {YourPrice = yourPrice, Tax = yourTax});
        }
    }
}