using System.Web;
using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Transaction.ViewModels;
using Sitecore.Data.Fields;
using Sitecore.Mvc.Presentation;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Web.UI.WebControls;

namespace AvenueClothing.Project.Transaction.Controllers
{
	public class ConfirmationController : BaseController
	{
		public ActionResult Rendering()
		{
            var confirmation= new ConfirmationViewModel();
            confirmation.Headline = new HtmlString(FieldRenderer.Render(RenderingContext.Current.ContextItem, "Headline"));
            confirmation.Message = new HtmlString(FieldRenderer.Render(RenderingContext.Current.ContextItem, "Message"));
            return View(confirmation);
		}
	}
}