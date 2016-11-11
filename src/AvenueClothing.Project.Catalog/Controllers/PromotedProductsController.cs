using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Project.Catalog.ViewModels;
using Sitecore.Mvc.Controllers;
using Sitecore.Mvc.Presentation;

namespace AvenueClothing.Project.Catalog.Controllers
{
	public class PromotedProductsController : SitecoreController
    {
		public ActionResult Rendering()
		{
			var promotedProductsViewModel = new PromotedProductsViewModel();

			promotedProductsViewModel.ProductGuids = RenderingContext.Current.ContextItem.Fields["Products"].ToString().Split('|').ToList();
			promotedProductsViewModel.ProductCardRendering = RenderingContext.Current.Rendering.DataSource;

			return View(promotedProductsViewModel);
		}
	}
}
