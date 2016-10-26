using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Feature.Catalog.Module.ViewModels;
using UCommerce.Api;
using UCommerce.Extensions;

namespace AvenueClothing.Feature.Catalog.Module.Controllers
{
    public class CategoryNavigationController : Controller
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