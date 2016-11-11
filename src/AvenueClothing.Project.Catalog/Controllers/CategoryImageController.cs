using System.Web;
using System.Web.Mvc;
using AvenueClothing.Project.Catalog.ViewModels;
using Sitecore.Mvc.Controllers;
using Sitecore.Mvc.Presentation;
using Sitecore.Web.UI.WebControls;

namespace AvenueClothing.Project.Catalog.Controllers
{
	public class CategoryImageController : SitecoreController
    {
		public ActionResult Rendering()
		{
			var categoryViewModel = new CategoryImageViewModel();
			
			categoryViewModel.Image = new HtmlString(FieldRenderer.Render(RenderingContext.Current.Rendering.Item, "Image"));

			return View(categoryViewModel);
		}
	}
}
