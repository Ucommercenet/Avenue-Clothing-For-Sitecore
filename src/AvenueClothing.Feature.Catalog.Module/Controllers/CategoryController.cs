using System.Collections.Generic;
using System.Web.Mvc;
using AvenueClothing.Feature.Catalog.Module.ViewModels;
using UCommerce.Api;
using UCommerce.Content;
using UCommerce.Extensions;
using UCommerce.Infrastructure;
using UCommerce.Runtime;
using UCommerce.Search.Facets;
using UCommerce.Search;

namespace AvenueClothing.Feature.Catalog.Module.Controllers
{
	public class CategoryController : Controller
	{
		public ActionResult Category()
		{
			var categoryViewModel = new CategoryViewModel();
			var currentCategory = SiteContext.Current.CatalogContext.CurrentCategory;

			categoryViewModel.Products = MapProducts(CatalogLibrary.GetProducts(currentCategory));

			return View("/views/Category.cshtml", categoryViewModel);
		}

		private IList<ProductViewModel> MapProducts(ICollection<UCommerce.EntitiesV2.Product> productsInCategory)
		{
			IList<ProductViewModel> productViews = new List<ProductViewModel>();

			foreach (UCommerce.EntitiesV2.Product product in productsInCategory)
			{
				var productViewModel = new ProductViewModel();

				productViewModel.Sku = product.Sku;
				productViewModel.Name = product.Name;
				productViewModel.Url = CatalogLibrary.GetNiceUrlForProduct(product);

				productViews.Add(productViewModel);
			}

			return productViews;
		}
	}

}
