using System.Linq;
using System.Web;
using System.Web.Mvc;
using AvenueClothing.Project.Catalog.ViewModels;
using AvenueClothing.Foundation.MvcExtensions;
using Sitecore.Mvc.Presentation;
using Sitecore.Web.UI.WebControls;
using Ucommerce;
using Ucommerce.Api;
using Ucommerce.Api.PriceCalculation;
using Ucommerce.Extensions;
using Ucommerce.Search;
using Ucommerce.Search.Extensions;
using Ucommerce.Search.Models;
using Ucommerce.Search.Slugs;

namespace AvenueClothing.Project.Catalog.Controllers
{
	public class ProductCardController : BaseController
    {
		private readonly ICatalogContext _catalogContext;
		private readonly IIndex<Product> _productIndex;
		private readonly IUrlService _urlService;
		public ProductCardController(ICatalogContext catalogContext, IIndex<Product> productIndex, IUrlService urlService)
		{
			_catalogContext = catalogContext;
			_productIndex = productIndex;
			_urlService = urlService;
		}

		public ActionResult Rendering()
		{
			var productView = new ProductCardRenderingViewModel();

            var database = Sitecore.Context.Database;
			var productItem = database.GetItem(RenderingContext.Current.Rendering.Properties["productGuid"]);

			if (productItem == null) return null;
            productView.DisplayName = new HtmlString(FieldRenderer.Render(productItem, "Display Name"));
			productView.ThumbnailImage = new HtmlString(FieldRenderer.Render(productItem, "Thumbnail image"));

			var currentProduct = _productIndex.Find().Where(x=>x.Guid == productItem.ID.Guid).SingleOrDefault();
			var category = _catalogContext.CurrentCategory;

			productView.Url = _urlService.GetUrl(_catalogContext.CurrentCatalog,
				_catalogContext.CurrentCategories.Append(_catalogContext.CurrentCategory).Compact()
				, currentProduct);

			productView.ProductPriceRenderingViewModel = GetProductPriceRenderingViewModel(currentProduct, category, _catalogContext.CurrentCatalog);

			return View(productView);
		}

		private ProductPriceRenderingViewModel GetProductPriceRenderingViewModel(Product currentProduct, Category currentCategory, ProductCatalog currentCatalog)
		{
			var taxRate = _catalogContext.CurrentPriceGroup.TaxRate;
			var currencyIsoCode = _catalogContext.CurrentPriceGroup.CurrencyISOCode;

			ProductPriceRenderingViewModel productPriceRenderingViewModelModel = new ProductPriceRenderingViewModel
			{
				CalculatePriceUrl = Url.Action("CalculatePrice", "ProductPrice"),
				CalculateVariantPriceUrl = Url.Action("CalculatePriceForVariant", "ProductPrice"),
				CatalogGuid = currentCatalog.Guid,
				Sku = currentProduct.Sku,
				ProductGuid = currentProduct.Guid
			};

			if (currentProduct.UnitPrices.TryGetValue(_catalogContext.CurrentPriceGroup.Name, out var unitPrice))
			{
				productPriceRenderingViewModelModel.Price = new Money(unitPrice * (1.0M + taxRate), currencyIsoCode).ToString();
				productPriceRenderingViewModelModel.Tax = new Money(unitPrice * taxRate, currencyIsoCode).ToString();
			}

			if (currentCategory != null)
			{
				productPriceRenderingViewModelModel.CategoryGuid = currentCategory.Guid;
			}

			return productPriceRenderingViewModelModel;
		}
    }
}
