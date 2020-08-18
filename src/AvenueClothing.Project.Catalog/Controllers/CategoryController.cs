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
using Ucommerce.Api;
using Ucommerce.Search;
using Ucommerce.Search.Extensions;
using Ucommerce.Search.Facets;
using Ucommerce.Search.Models;

namespace AvenueClothing.Project.Catalog.Controllers
{
	public class CategoryController : BaseController
    {
		private readonly ICatalogContext _catalogContext;
		private readonly ICatalogLibrary _catalogLibrary;
		private readonly IIndex<Product> _productIndex;

		public CategoryController(ICatalogContext catalogContext, ICatalogLibrary catalogLibrary, IIndex<Product> productIndex)
		{
			_catalogContext = catalogContext;
			_catalogLibrary = catalogLibrary;
			_productIndex = productIndex;
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

            FacetModelBinder facetBinder = new FacetModelBinder();
            IList<Facet> facetsForQuerying = (IList<Facet>)facetBinder.BindModel(new ControllerContext(), new ModelBindingContext());

			var productGuidsInCategory = new List<Guid>();
			var subCategories = _catalogLibrary.GetCategories(category.Categories);

			foreach (var subcategory in subCategories)
			{
				productGuidsInCategory.AddRange(GetProductGuidsInFacetsAndSelectedProductOnSitecoreItem(subcategory));
			}

		    if (RenderingContext.Current.ContextItem.Fields["Products"].ToString() != "")
		    {
			    var productsInCategory = _catalogLibrary.GetProducts(category.Guid, facetsForQuerying.ToFacetDictionary()).Select(x=>x.Guid);

		        var selectedProductItems =
		            RenderingContext.Current.ContextItem.Fields["Products"].ToString()
		                .Split('|')
		                .Select(x => new Guid(x))
		                .ToList();

		        var productGuids = _catalogLibrary.GetProducts(category.Guid)
		            .Where(x => selectedProductItems.Contains(x.Guid))
		            .Where(x => productsInCategory.Contains(x.Guid))
		            .Select(x => x.Guid)
		            .ToList();

		        productGuidsInCategory.AddRange(productGuids);
		    }

		    return productGuidsInCategory;
		}
	}
}
