using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Feature.Catalog.Module.ViewModels;
using Sitecore.Mvc.Controllers;
using Sitecore.Mvc.Presentation;

namespace AvenueClothing.Feature.Catalog.Module.Controllers
{
	public class PromotedProductsController : SitecoreController
    {
		public ActionResult Rendering()
		{
			var promotedProductsViewModel = new PromotedProductsViewModel();
			promotedProductsViewModel.ProductGuids = RenderingContext.Current.ContextItem.Fields["Products"].ToString().Split('|').ToList();
			return View(promotedProductsViewModel);
		}
	}
}
