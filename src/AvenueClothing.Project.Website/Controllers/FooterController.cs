using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;

namespace AvenueClothing.Project.Website.Controllers
{
    public class FooterController: BaseController
    {

        public ActionResult Rendering()
        {
            return View();
        }
    }
}