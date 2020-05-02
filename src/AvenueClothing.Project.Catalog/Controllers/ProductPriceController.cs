using System.Web.Mvc;
using AvenueClothing.Project.Catalog.ViewModels;
using AvenueClothing.Foundation.MvcExtensions;
using Ucommerce.Api;

namespace AvenueClothing.Project.Catalog.Controllers
{
	public class ProductPriceController : BaseController
    {
        private readonly ICatalogContext _catalogContext;

        public ProductPriceController(ICatalogContext catalogContext)
        {
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
   //          var taxRate = _catalogContext.CurrentPriceGroup.TaxRate;
   //          var currencyIsoCode = _catalogContext.CurrentPriceGroup.CurrencyISOCode;
   //
   //          var product = _productRepository.Select(x => x.Sku == priceCalculationDetails.ProductSku && x.ParentProduct == null).FirstOrDefault();
   //          var catalog = _catalogLibraryInternal.GetCatalog(priceCalculationDetails.CatalogId);
   //          PriceCalculation priceCalculation = new PriceCalculation(product, catalog);
   //
   //          var yourPrice = priceCalculation.YourPrice.Amount.ToString();
   //          var yourTax = priceCalculation.YourTax.ToString();
			// var discount = priceCalculation.Discount.Amount.ToString();
   //
   //          return Json(new {YourPrice = yourPrice, Tax = yourTax, Discount = discount});
            return Json("");
        }

        [HttpPost]
		public ActionResult CalculatePriceForVariant(ProductPriceCalculatePriceForVariantViewModel variantPriceCalculationDetails)
        {
            // Product variant = _productRepository.Select(x => x.VariantSku == variantPriceCalculationDetails.ProductVariantSku && x.Sku == variantPriceCalculationDetails.ProductSku).FirstOrDefault();
            // var catalog = _catalogLibraryInternal.GetCatalog(variantPriceCalculationDetails.CatalogId);
            // PriceCalculation priceCalculation = new PriceCalculation(variant, catalog);
            //
            // var yourPrice = priceCalculation.YourPrice.Amount.ToString();
            // var yourTax = priceCalculation.YourTax.ToString();
            //
            // return Json(new {YourPrice = yourPrice, Tax = yourTax});
            return Json("");
        }
    }
}