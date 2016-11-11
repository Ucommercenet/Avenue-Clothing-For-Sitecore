using System.Collections.Generic;
using System.Web.Mvc;
using AvenueClothing.Project.Catalog.ViewModels;
using AvenueClothing.Foundation.MvcExtensions;
using UCommerce.Api;
using UCommerce.Extensions;

namespace AvenueClothing.Project.Catalog.Controllers
{
	public class CategoryNavigationController : BaseController
    {
		public ActionResult Rendering()
		{
		    var categoryNavigation = new CategoryNavigationRenderingViewModel
		    {
		        Categories = MapCategories(CatalogLibrary.GetRootCategories())
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

				categoriesToReturn.Add(categoryToAdd);

				categoryToAdd.Categories = MapCategories(CatalogLibrary.GetCategories(category));
			}

			return categoriesToReturn;
		} 
    }
}