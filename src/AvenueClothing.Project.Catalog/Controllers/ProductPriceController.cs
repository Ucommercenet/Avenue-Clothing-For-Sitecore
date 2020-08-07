using System.Web.Mvc;
using AvenueClothing.Project.Catalog.ViewModels;
using AvenueClothing.Foundation.MvcExtensions;
using Ucommerce;
using Ucommerce.Api;
using Ucommerce.Search;
using Ucommerce.Search.Models;

namespace AvenueClothing.Project.Catalog.Controllers
{
	public class ProductPriceController : BaseController
    {
        private readonly ICatalogContext _catalogContext;
        private readonly IIndex<Product> _productIndex;

        public ProductPriceController(ICatalogContext catalogContext, IIndex<Product> productIndex)
        {
            _catalogContext = catalogContext;
            _productIndex = productIndex;
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
            var product = _productIndex.Find().Where(x => x.Sku == priceCalculationDetails.ProductSku && x.VariantSku == null).SingleOrDefault();

            product.UnitPrices.TryGetValue(_catalogContext.CurrentPriceGroup.Name, out decimal unitPrice);
            string currencyIsoCode = _catalogContext.CurrentPriceGroup.CurrencyISOCode;
            decimal taxRate = _catalogContext.CurrentPriceGroup.TaxRate;

            var yourTax = unitPrice > 0 ? new Money(unitPrice * taxRate, currencyIsoCode).ToString() : "";
            var yourPrice = unitPrice > 0 ? new Money(unitPrice * (1.0M + taxRate), currencyIsoCode).ToString() : "";

            return Json(new {YourPrice = yourPrice, Tax = yourTax, Discount = 0});
        }

        [HttpPost]
		public ActionResult CalculatePriceForVariant(ProductPriceCalculatePriceForVariantViewModel variantPriceCalculationDetails)
        {
            var product = _productIndex.Find().Where(x =>
                x.Sku == variantPriceCalculationDetails.ProductSku &&
                x.VariantSku == variantPriceCalculationDetails.ProductVariantSku).SingleOrDefault();

            product.UnitPrices.TryGetValue(_catalogContext.CurrentPriceGroup.Name, out decimal unitPrice);
            string currencyIsoCode = _catalogContext.CurrentPriceGroup.CurrencyISOCode;
            decimal taxRate = _catalogContext.CurrentPriceGroup.TaxRate;

            var yourTax = unitPrice > 0 ? new Money(unitPrice * taxRate, currencyIsoCode).ToString() : "";
            var yourPrice = unitPrice > 0 ? new Money(unitPrice * (1.0M + taxRate), currencyIsoCode).ToString() : "";

             return Json(new {YourPrice = yourPrice, Tax = yourTax});
        }
    }
}