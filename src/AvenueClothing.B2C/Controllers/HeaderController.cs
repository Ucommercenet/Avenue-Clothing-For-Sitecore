using System.Web.Mvc;

namespace AvenueClothing.Project.Website.Controllers
{
    public class HeaderController : Controller
    {
		public ActionResult Header()
		{
			return View("/views/Avenue Clothing/header.cshtml");
		}
    }
}