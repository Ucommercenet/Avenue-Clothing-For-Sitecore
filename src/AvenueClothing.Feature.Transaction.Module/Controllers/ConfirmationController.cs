using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;

namespace AvenueClothing.Feature.Transaction.Module.Controllers
{
	public class ConfirmationController : BaseController
	{
		public ActionResult Rendering()
		{
			return View();
		}
	}
}