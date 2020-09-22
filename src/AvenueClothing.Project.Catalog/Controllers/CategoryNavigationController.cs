using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Project.Catalog.ViewModels;
using AvenueClothing.Foundation.MvcExtensions;
using Ucommerce.Api;
using Ucommerce.Search.Models;
using Ucommerce.Search.Slugs;

namespace AvenueClothing.Project.Catalog.Controllers
{
	public class CategoryNavigationController : BaseController
	{
	    private readonly ICatalogLibrary _catalogLibrary;
	    private readonly IUrlService _urlService;
	    private readonly ICatalogContext _catalogContext;

	    public CategoryNavigationController(ICatalogLibrary catalogLibrary, IUrlService urlService, ICatalogContext catalogContext)
	    {
	        _catalogLibrary = catalogLibrary;
	        _urlService = urlService;
	        _catalogContext = catalogContext;
	    }

		public ActionResult Rendering()
		{
			var categoryNavigation = new CategoryNavigationRenderingViewModel();

			IEnumerable<Category> rootCategories = _catalogLibrary.GetRootCategories().ToList();

			categoryNavigation.Categories = MapCategories(rootCategories);


			return View(categoryNavigation);
		}

		private List<CategoryNavigationRenderingViewModel.Category> MapCategories(IEnumerable<Category> categoriesToMap)
		{
            var categoriesToReturn = new List<CategoryNavigationRenderingViewModel.Category>();

            var allSubCategoryIds = categoriesToMap.SelectMany(cat => cat.Categories).Distinct().ToList();
            var subCategoriesById = _catalogLibrary.GetCategories(allSubCategoryIds).ToDictionary(cat => cat.Guid);

			foreach (var category in categoriesToMap)
			{
				var categoryToAdd = new CategoryNavigationRenderingViewModel.Category
				{
					Name = category.DisplayName,
					Url = _urlService.GetUrl(_catalogContext.CurrentCatalog, new[] {category}),
					Categories = category.Categories
						.Where(id => subCategoriesById.ContainsKey(id))
						.Select(id => subCategoriesById[id])
						.Select(cat => new CategoryNavigationRenderingViewModel.Category
						{
							Name = cat.DisplayName,
							Url = _urlService.GetUrl(_catalogContext.CurrentCatalog, new[] {category, cat})
						})
						.ToList()
				};

				categoriesToReturn.Add(categoryToAdd);
			}

			return categoriesToReturn;
		}
    }
}