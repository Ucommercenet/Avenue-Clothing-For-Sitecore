using System.Web.Mvc;
using Sitecore.Mvc.Controllers;

namespace AvenueClothing.Project.Website.Controllers
{
    public class HeaderController : SitecoreController
    {
		public ActionResult Rendering()
		{
			return View();
		}
    }
}