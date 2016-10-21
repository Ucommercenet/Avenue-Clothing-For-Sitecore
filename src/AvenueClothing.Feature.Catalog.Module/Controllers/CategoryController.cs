using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Feature.Catalog.Module.Extensions;
using AvenueClothing.Feature.Catalog.Module.ViewModels;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using UCommerce.Api;
using UCommerce.Content;
using UCommerce.EntitiesV2;
using UCommerce.Extensions;
using UCommerce.Infrastructure;
using UCommerce.Runtime;
using UCommerce.Search.Facets;

namespace AvenueClothing.Feature.Catalog.Module.Controllers
{
	public class CategoryController : Controller
	{
		private readonly ICatalogContext _catalogContext;

		public CategoryController(ICatalogContext catalogContext)
		{
			_catalogContext = catalogContext;
		}

		public ActionResult Category()
		{
			var categoryViewModel = new CategoryViewModel();
			var currentCategory = _catalogContext.CurrentCategory;
			var productGuidsFromFacets = GetProductGuidsInFacets(currentCategory);

			categoryViewModel.ProductItemGuids = RemoveUnselectedSitecoreItems(productGuidsFromFacets, RenderingContext.Current.ContextItem);

			return View("/views/Category.cshtml", categoryViewModel);
		}

		private IList<Guid> RemoveUnselectedSitecoreItems(IList<Guid> productGuidsFromFacets, Item contextItem)
		{
			IList<Guid> selectedProductItems = contextItem.Fields["Products"].ToString().Split('|').Select(x => new Guid(x)).ToList();
			return productGuidsFromFacets.Where(x => selectedProductItems.Contains(x)).ToList();
		}

		private IList<Guid> GetProductGuidsInFacets(Category category)
		{
			IList<Facet> facetsForQuerying = System.Web.HttpContext.Current.Request.QueryString.ToFacets();
			var productGuidsInCategory = new List<Guid>();

			foreach (var subcategory in category.Categories)
			{
				productGuidsInCategory.AddRange(GetProductGuidsInFacets(subcategory));
			}

			IEnumerable<int> productIds = SearchLibrary.GetProductsFor(category, facetsForQuerying).Select(x => x.Id);
		
			var productGuids =CatalogLibrary.GetProducts(category).Where(x => productIds.Contains(x.ProductId)).Select(x => x.Guid).ToList();
			productGuidsInCategory.AddRange(productGuids);

			return productGuidsInCategory;
		}
	}
}
