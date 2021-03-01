using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Project.Catalog.ViewModels;
using AvenueClothing.Foundation.MvcExtensions;
using Ucommerce;
using Ucommerce.Api;
using Ucommerce.Catalog;
using Ucommerce.Catalog.Models;
using Ucommerce.Search;
using Ucommerce.Search.Models;

namespace AvenueClothing.Project.Catalog.Controllers
{
    public class ProductPriceController : BaseController
    {
        private readonly ICatalogContext _catalogContext;
        private readonly IIndex<Product> _productIndex;
        private readonly IProductPriceCalculationService _productPriceCalculationService;

        public ProductPriceController(ICatalogContext catalogContext, IIndex<Product> productIndex,
            IProductPriceCalculationService productPriceCalculationService)
        {
            _catalogContext = catalogContext;
            _productIndex = productIndex;
            _productPriceCalculationService = productPriceCalculationService;
        }

        public ActionResult Rendering()
        {
            var currentProduct = _catalogContext.CurrentProduct;
            var currentCategory = _catalogContext.CurrentCategory;
            var currentCatalog = _catalogContext.CurrentCatalog;

            ProductPriceRenderingViewModel productPriceRenderingViewModelModel = new ProductPriceRenderingViewModel()
            {
                CalculatePriceUrl = Url.Action("CalculatePrice"),
                CalculateVariantPriceUrl = Url.Action("CalculatePriceForVariant"),
                CatalogGuid = currentCatalog.Guid,
                Sku = currentProduct.Sku,
                ProductGuid = currentProduct.Guid
            };
            if (currentCategory != null)
            {
                productPriceRenderingViewModelModel.CategoryGuid = currentCategory.Guid;
            }

            return View(productPriceRenderingViewModelModel);
        }

        [HttpPost]
        public ActionResult CalculatePrice(ProductCardRenderingViewModel priceCalculationDetails)
        {
            var product = _productIndex.Find()
                .Where(x => x.Sku == priceCalculationDetails.ProductSku && x.VariantSku == null).SingleOrDefault();

            string priceGroupName = _catalogContext.CurrentPriceGroup.Name;
            string currencyIsoCode = _catalogContext.CurrentPriceGroup.CurrencyISOCode;

            var yourPrice = new Money(product.PricesInclTax[priceGroupName], currencyIsoCode).ToString();
            var yourTax = new Money(product.Taxes[priceGroupName], currencyIsoCode).ToString();

            return Json(new {YourPrice = yourPrice, Tax = yourTax, Discount = 0});
        }

        [HttpPost]
        public ActionResult CalculatePriceForVariant(
            ProductPriceCalculatePriceForVariantViewModel variantPriceCalculationDetails)
        {
            var product = _productIndex.Find().Where(x =>
                x.Sku == variantPriceCalculationDetails.ProductSku &&
                x.VariantSku == variantPriceCalculationDetails.ProductVariantSku).SingleOrDefault();

            string priceGroupName = _catalogContext.CurrentPriceGroup.Name;
            string currencyIsoCode = _catalogContext.CurrentPriceGroup.CurrencyISOCode;

            var yourPrice = new Money(product.PricesInclTax[priceGroupName], currencyIsoCode).ToString();
            var yourTax = new Money(product.Taxes[priceGroupName], currencyIsoCode).ToString();

            return Json(new {YourPrice = yourPrice, Tax = yourTax});
        }
    }
}