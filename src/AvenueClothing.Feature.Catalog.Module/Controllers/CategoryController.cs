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
		public ActionResult Category()
		{
			var categoryViewModel = new CategoryViewModel();
			var currentCategory = SiteContext.Current.CatalogContext.CurrentCategory;
			var productGuidsFromUCommerce = MapProductsInCategories(currentCategory);

			categoryViewModel.ProductItemGuids = RemoveUnselectedSitecoreItems(productGuidsFromUCommerce, RenderingContext.Current.ContextItem);

			return View("/views/Category.cshtml", categoryViewModel);
		}

		private IList<Guid> RemoveUnselectedSitecoreItems(IList<Guid> productGuidsFromUCommerce, Item contextItem)
		{
			IList<Guid> selectedProductItems = contextItem.Fields["Products"].ToString().Split('|').Select(x => new Guid(x)).ToList();
			return productGuidsFromUCommerce.Where(x => selectedProductItems.Contains(x)).ToList();
		}

		private IList<Guid> MapProductsInCategories(Category category)
		{
			IList<Facet> facetsForQuerying = System.Web.HttpContext.Current.Request.QueryString.ToFacets();
			var productGuidsInCategory = new List<Guid>();

			foreach (var subcategory in category.Categories)
			{
				productGuidsInCategory.AddRange(MapProductsInCategories(subcategory));
			}

			var productIds = SearchLibrary.GetProductsFor(category, facetsForQuerying).Select(x => x.Id);
			var productRepository = ObjectFactory.Instance.Resolve<IRepository<Product>>();
			productGuidsInCategory.AddRange(productRepository.Select(x => productIds.Contains(x.Id)).Select(x => x.Guid).ToList());

			return productGuidsInCategory;
		}
	}
}
