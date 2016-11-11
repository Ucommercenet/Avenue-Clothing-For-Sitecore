using System.Web;
using System.Web.Mvc;
using AvenueClothing.Project.Catalog.ViewModels;
using Sitecore.Mvc.Controllers;
using Sitecore.Mvc.Presentation;
using Sitecore.Web.UI.WebControls;

namespace AvenueClothing.Project.Catalog.Controllers
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
