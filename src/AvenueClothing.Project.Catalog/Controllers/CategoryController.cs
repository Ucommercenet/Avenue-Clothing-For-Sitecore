using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AvenueClothing.Project.Catalog.ViewModels;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Catalog.Services;
using Sitecore.Mvc.Presentation;
using Sitecore.Web.UI.WebControls;
using UCommerce.Catalog;
using UCommerce.EntitiesV2;
using UCommerce.Extensions;
using UCommerce.Runtime;
using UCommerce.Search;
using UCommerce.Search.Facets;

namespace AvenueClothing.Project.Catalog.Controllers
{
	public class CategoryController : BaseController
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

		public ActionResult Rendering()
		{
			var categoryViewModel = new CategoryRenderingViewModel();

			var currentCategory = _catalogContext.CurrentCategory;

			categoryViewModel.DisplayName = new HtmlString(FieldRenderer.Render(RenderingContext.Current.ContextItem, "Display name"));

			categoryViewModel.ProductItemGuids = GetProductGuidsInFacetsAndSelectedProductOnSitecoreItem(currentCategory);

			categoryViewModel.ProductCardRendering = RenderingContext.Current.Rendering.DataSource;

			return View(categoryViewModel);
		}
		
		private List<Guid> GetProductGuidsInFacetsAndSelectedProductOnSitecoreItem(Category category)
		{

            FacetModelBinder FacetBinder = new FacetModelBinder();
            IList<Facet> facetsForQuerying = (IList<Facet>)FacetBinder.BindModel(new ControllerContext(), new ModelBindingContext());

			var productGuidsInCategory = new List<Guid>();

			foreach (var subcategory in category.Categories)
			{
				productGuidsInCategory.AddRange(GetProductGuidsInFacetsAndSelectedProductOnSitecoreItem(subcategory));
			}

		    if (RenderingContext.Current.ContextItem.Fields["Products"].ToString() != "")
		    {
		        var productIds = _searchLibraryInternal.GetProductsFor(category, facetsForQuerying).Select(x => x.Id);

		        var selectedProductItems =
		            RenderingContext.Current.ContextItem.Fields["Products"].ToString()
		                .Split('|')
		                .Select(x => new Guid(x))
		                .ToList();

		        var productGuids = _catalogLibraryInternal.GetProductsInCategory(category)
		            .Where(x => selectedProductItems.Contains(x.Guid))
		            .Where(x => productIds.Contains(x.ProductId))
		            .Select(x => x.Guid)
		            .ToList();

		        productGuidsInCategory.AddRange(productGuids);
		    }

		    return productGuidsInCategory;
		}
	}
}
