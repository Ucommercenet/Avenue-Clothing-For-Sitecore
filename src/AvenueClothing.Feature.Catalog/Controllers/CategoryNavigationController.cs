using System.Collections.Generic;
using System.Web.Mvc;
using AvenueClothing.Project.Catalog.ViewModels;
using AvenueClothing.Foundation.MvcExtensions;
using UCommerce.Api;
using UCommerce.Catalog;
using UCommerce.Extensions;

namespace AvenueClothing.Project.Catalog.Controllers
{
	public class CategoryNavigationController : BaseController
	{
	    private readonly CatalogLibraryInternal _catalogLibraryInternal;

	    public CategoryNavigationController(CatalogLibraryInternal catalogLibraryInternal)
	    {
	        _catalogLibraryInternal = catalogLibraryInternal;
	    }

		public ActionResult Rendering()
		{
		    var categoryNavigation = new CategoryNavigationRenderingViewModel
		    {
		        Categories = MapCategories(_catalogLibraryInternal.GetRootCategories())
		    };
            

			return View(categoryNavigation);
		}

		private List<CategoryNavigationRenderingViewModel.Category> MapCategories(ICollection<UCommerce.EntitiesV2.Category> categoriesToMap)
		{
            var categoriesToReturn = new List<CategoryNavigationRenderingViewModel.Category>();

			foreach (UCommerce.EntitiesV2.Category category in categoriesToMap)
			{
				var categoryToAdd = new CategoryNavigationRenderingViewModel.Category();

				categoryToAdd.Name = category.DisplayName();
			    categoryToAdd.Url = CatalogLibrary.GetNiceUrlForCategory(category);

			    categoryToAdd.Url = _catalogLibraryInternal.GetNiceUrlForCategory(null, category);

				categoriesToReturn.Add(categoryToAdd);

				categoryToAdd.Categories = MapCategories(_catalogLibraryInternal.GetCategories(category));
			}

			return categoriesToReturn;
		} 
    }
}