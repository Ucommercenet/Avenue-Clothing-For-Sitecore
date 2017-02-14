using System.Web;
using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Feature.Catalog.ViewModels;
using Sitecore.Mvc.Presentation;
using Sitecore.Web.UI.WebControls;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.Runtime;

namespace AvenueClothing.Feature.Catalog.Controllers
{
	public class ProductCardController : BaseController
    {
		private readonly IRepository<Product> _productRepository;
		private readonly ICatalogContext _catalogContext;

		public ProductCardController(IRepository<Product> productRepository, ICatalogContext catalogContext)
		{
			_productRepository = productRepository;
			_catalogContext = catalogContext;
		}

		public ActionResult Rendering()
		{
			var productView = new ProductCardRenderingViewModel();

            var database = Sitecore.Context.Database;
			var productItem = database.GetItem(RenderingContext.Current.Rendering.Properties["productGuid"]);
            
			if (productItem == null) return null;
            productView.DisplayName = new HtmlString(FieldRenderer.Render(productItem, "Display Name"));
			productView.ThumbnailImage = new HtmlString(FieldRenderer.Render(productItem, "Thumbnail image"));

			var currentProduct = _productRepository.SingleOrDefault(x => x.Guid == productItem.ID.Guid);
			var category = _catalogContext.CurrentCategory;

			productView.Url = CatalogLibrary.GetNiceUrlForProduct(currentProduct, category);
			productView.ProductPriceRenderingViewModel = GetProductPriceRenderingViewModel(currentProduct, category, _catalogContext.CurrentCatalog);
			
			return View(productView);
		}

		private ProductPriceRenderingViewModel GetProductPriceRenderingViewModel(Product currentProduct, Category currentCategory, ProductCatalog currentCatalog)
		{
			ProductPriceRenderingViewModel productPriceRenderingViewModelModel = new ProductPriceRenderingViewModel()
			{
				CalculatePriceUrl = Url.Action("CalculatePrice", "ProductPrice"),
				CalculateVariantPriceUrl = Url.Action("CalculatePriceForVariant", "ProductPrice"),
				CatalogGuid = currentCatalog.Id,
				Sku = currentProduct.Sku,
				ProductId = currentProduct.Id
			};

			if (currentCategory != null)
			{
				productPriceRenderingViewModelModel.CategoryGuid = currentCategory.Guid;
			}

			return productPriceRenderingViewModelModel;
		}
    }
}
