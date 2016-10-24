using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Feature.Catalog.Module.Extensions;
using AvenueClothing.Feature.Catalog.Module.ViewModels;
using Sitecore.Mvc.Presentation;
using UCommerce.Api;
using UCommerce.Catalog;
using UCommerce.EntitiesV2;
using UCommerce.Runtime;
using UCommerce.Search;

namespace AvenueClothing.Feature.Catalog.Module.Controllers
{
	public class CategoryController : Controller
	{
		private readonly ICatalogContext _catalogContext;
		private readonly CatalogLibraryInternal _catalogLibraryInternal;
		private readonly SearchLibraryInternal _searchLibraryInternal;

		public CategoryController(ICatalogContext catalogContext, CatalogLibraryInternal catalogLibraryInternal, SearchLibraryInternal searchLibraryInternal)
		{
			_catalogContext = catalogContext;
			_catalogLibraryInternal = catalogLibraryInternal;
			_searchLibraryInternal = searchLibraryInternal;
		}

		public ActionResult Category()
		{
			var categoryViewModel = new CategoryViewModel();

			var currentCategory = _catalogContext.CurrentCategory;

			categoryViewModel.ProductItemGuids = GetProductGuidsInFacetsAndSelectedProductOnSitecoreItem(currentCategory);

			return View(categoryViewModel);
		}
		
		private IList<Guid> GetProductGuidsInFacetsAndSelectedProductOnSitecoreItem(Category category)
		{
			var facetsForQuerying = HttpContext.Request.QueryString.ToFacets();
			var productGuidsInCategory = new List<Guid>();

			foreach (var subcategory in category.Categories)
			{
				productGuidsInCategory.AddRange(GetProductGuidsInFacetsAndSelectedProductOnSitecoreItem(subcategory));
			}

			var productIds = _searchLibraryInternal.GetProductsFor(category, facetsForQuerying).Select(x => x.Id);

			var selectedProductItems = RenderingContext.Current.ContextItem.Fields["Products"].ToString().Split('|').Select(x => new Guid(x)).ToList();

			var productGuids = _catalogLibraryInternal.GetProductsInCategory(category)
				.Where(x => selectedProductItems.Contains(x.Guid))
				.Where(x => productIds.Contains(x.ProductId))
				.Select(x => x.Guid)
				.ToList();

			productGuidsInCategory.AddRange(productGuids);

			return productGuidsInCategory;
		}
	}
}
