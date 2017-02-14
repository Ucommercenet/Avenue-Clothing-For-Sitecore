using System.Web;
using System.Web.Mvc;
using AvenueClothing.Project.Catalog.ViewModels;
using Sitecore.Mvc.Controllers;
using Sitecore.Mvc.Presentation;
using Sitecore.Web.UI.WebControls;

namespace AvenueClothing.Project.Catalog.Controllers
{
	public class ProductPrimaryImageController : SitecoreController
    {
		public ActionResult Rendering()
		{
			var productPrimaryImageViewModel = new ProductPrimaryImageViewModel();

			productPrimaryImageViewModel.PrimaryImage = new HtmlString(FieldRenderer.Render(RenderingContext.Current.Rendering.Item, "Primary image"));

			return View(productPrimaryImageViewModel);
		}
	}
}
