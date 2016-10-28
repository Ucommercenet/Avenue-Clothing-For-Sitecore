using System.Web;
using System.Web.Mvc;
using AvenueClothing.Feature.Catalog.Module.ViewModels;
using Sitecore.Mvc.Controllers;
using Sitecore.Mvc.Presentation;
using Sitecore.Web.UI.WebControls;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.Runtime;

namespace AvenueClothing.Feature.Catalog.Module.Controllers
{
	public class ProductCardController : SitecoreController
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
			var productItem = database.GetItem(RenderingContext.Current.Rendering.Properties["productItem"]);
            
			if (productItem == null) return null;
            productView.DisplayName = new HtmlString(FieldRenderer.Render(productItem, "Display Name"));

            RenderingContext.Current.ContextItem = productItem;

			var currentProduct = _productRepository.SingleOrDefault(x => x.Guid == productItem.ID.Guid);
			var category = _catalogContext.CurrentCategory;

			productView.Url = CatalogLibrary.GetNiceUrlForProduct(currentProduct, category);
			productView.Amount = CatalogLibrary.CalculatePrice(currentProduct).YourPrice.Amount.ToString();
			
			return View(productView);
		}
	}
}
