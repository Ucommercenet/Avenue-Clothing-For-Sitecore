using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AvenueClothing.Feature.Catalog.Module.ViewModels;
using UCommerce.Api;
using UCommerce.Catalog;
using UCommerce.EntitiesV2;
using UCommerce.Runtime;

namespace AvenueClothing.Feature.Catalog.Module.Controllers
{
    public class ProductPriceController:Controller
    {
        private readonly IRepository<Product> _productRepository;
        private readonly CatalogLibraryInternal _catalogLibraryInternal;
        private readonly ICatalogContext _catalogContext;

        public ProductPriceController(IRepository<Product> productRepository, CatalogLibraryInternal catalogLibraryInternal, ICatalogContext catalogContext)
        {
            _productRepository = productRepository;
            _catalogLibraryInternal = catalogLibraryInternal;
            _catalogContext = catalogContext;
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
                CatalogGuid = currentCatalog.Id,
                SKU = currentProduct.Sku
            };
            if (currentCategory != null)
            {
                productPriceRenderingViewModelModel.CategoryGuid = currentCategory.Guid;
            }
            
            return View(productPriceRenderingViewModelModel);
        }

        [HttpPost]
        public ActionResult CalculatePrice(PriceCalculationDetails priceCalculationDetails)
        {
            var product = _productRepository.Select(x => x.Sku == priceCalculationDetails.ProductSku && x.ParentProduct == null).FirstOrDefault();
            var catalog = _catalogLibraryInternal.GetCatalog(priceCalculationDetails.CatalogId);
            PriceCalculation priceCalculation = new PriceCalculation(product, catalog);

            var yourPrice = priceCalculation.YourPrice.Amount.ToString();
            var yourTax = priceCalculation.YourTax.ToString();

            return Json(new {YourPrice = yourPrice, Tax = yourTax});
        }

        [HttpPost]
        public ActionResult CalculatePriceForVariant(VariantPriceCalculationDetails variantPriceCalculationDetails)
        {
            Product variant = _productRepository.Select(x => x.VariantSku == variantPriceCalculationDetails.ProductVariantSku && x.Sku == variantPriceCalculationDetails.ProductSku).FirstOrDefault();
            var catalog = _catalogLibraryInternal.GetCatalog(variantPriceCalculationDetails.CatalogId);
            PriceCalculation priceCalculation = new PriceCalculation(variant, catalog);

            var yourPrice = priceCalculation.YourPrice.Amount.ToString();
            var yourTax = priceCalculation.YourTax.ToString();

            return Json(new {YourPrice = yourPrice, Tax = yourTax});
        }
    }
}