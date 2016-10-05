using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Feature.Catalog.Module.Extensions;
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

			var facets = System.Web.HttpContext.Current.Request.QueryString.ToFacets();

			categoryViewModel.ProductIds = SearchLibrary.GetProductsFor(currentCategory, facets).Select(x => x.Id).ToList();

			return View("/views/Category.cshtml", categoryViewModel);
		}
	}
}
