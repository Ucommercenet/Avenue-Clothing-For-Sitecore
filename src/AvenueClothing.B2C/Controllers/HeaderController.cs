using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensionsModule;

namespace AvenueClothing.Project.Website.Controllers
{
    public class HeaderController : BaseController
    {
		public ActionResult Rendering()
		{
			return View();
		}
    }
}