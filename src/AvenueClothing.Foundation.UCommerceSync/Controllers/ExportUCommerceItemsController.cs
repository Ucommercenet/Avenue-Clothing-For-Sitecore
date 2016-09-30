using System;
using System.Web.Mvc;
using UCommerceSync;
using UCommerceSync.Logging;

namespace AvenueClothing.Foundation.UCommerceSync.Controllers
{
	public class ExportUCommerceItemsController : Controller
    {
		public ActionResult Export()
		{
			var logger = new DefaultLogger();
			try
			{
				var settings = new ExportSettings
				{
					ExportProducts = false // you probably don't want to export product data
				};
				var exporter = new Exporter(settings, @"C:\Projects\Avenue Clothing for Sitecore\src\project\AvenueClothing\uCommerceSync", logger);
				exporter.Export();
			}
			catch (Exception ex)
			{
				logger.Error(ex, "An unexpected error occurred during export");
			}

			return View("/views/ExportUCommerceItems.cshtml");
		}
    }
}