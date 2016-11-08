using System.Web.Mvc;
using Sitecore.Mvc.Controllers;

namespace AvenueClothing.Feature.Transaction.Module.Controllers
{
	public class ConfirmationController : SitecoreController
	{
		public ActionResult Rendering()
		{
			return View();
		}
	}
}