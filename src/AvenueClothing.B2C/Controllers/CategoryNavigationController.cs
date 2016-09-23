using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AvenueClothing.Project.Website.ViewModels;
using UCommerce.Extensions;

namespace AvenueClothing.Project.Website.Controllers
{
    public class CategoryNavigationController : Controller
    {
		public ActionResult CategoryNavigation()
		{
			var categoryNavigation = new CategoryNavigationViewModel();

			categoryNavigation.Categories = MapCategories(UCommerce.Api.CatalogLibrary.GetRootCategories());

			return View("/views/PartialViews/CategoryNavigation.cshtml", categoryNavigation);
		}

		private IList<CategoryViewModel> MapCategories(ICollection<UCommerce.EntitiesV2.Category> categoriesToMap)
		{
			var categoriesToReturn = new List<CategoryViewModel>();

			foreach (UCommerce.EntitiesV2.Category category in categoriesToMap)
			{
				var categoryToAdd = new CategoryViewModel();

				categoryToAdd.Name = category.DisplayName();
				categoryToAdd.Url = "/store/category?category=" + category.CategoryId;

				categoriesToReturn.Add(categoryToAdd);

				categoryToAdd.Categories = MapCategories(UCommerce.Api.CatalogLibrary.GetCategories(category));
			}

			return categoriesToReturn;
		} 
    }
}