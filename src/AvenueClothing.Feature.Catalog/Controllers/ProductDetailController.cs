using System.Web;
using System.Web.Mvc;
using AvenueClothing.Feature.Catalog.ViewModels;
using Sitecore.Mvc.Controllers;
using Sitecore.Mvc.Presentation;
using Sitecore.Web.UI.WebControls;

namespace AvenueClothing.Feature.Catalog.Controllers
{
	public class ProductDetailController : SitecoreController
	{
		public ActionResult Rendering()
		{
			var productDetailViewModel = new ProductDetailViewModel();

			productDetailViewModel.LongDescription = new HtmlString(FieldRenderer.Render(RenderingContext.Current.ContextItem, "Long description"));

			return View(productDetailViewModel);
		}
	}
}
