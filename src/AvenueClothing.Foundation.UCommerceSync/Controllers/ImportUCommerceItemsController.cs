using System;
using System.Web.Mvc;
using UCommerceSync;
using UCommerceSync.Logging;

namespace AvenueClothing.Foundation.UCommerceSync.Controllers
{
	public class ImportUCommerceItemsController : Controller
    {
		public ActionResult Import()
		{
			var logger = new DefaultLogger();
			try
			{
				var settings = new ImportSettings
				{
					ImportProductCatalogGroups = true,
					ImportProductCatalogs = true,
					ImportProductCategories = true,
					ImportProducts = false, // you probably don't want to import product data
					ImportMarketingFoundation = false, // you might not want to import marketing foundation settins either
					PerformCleanUp = true // set to false if you don't want to delete stuff
				};
				var importer = new Importer(settings, @"C:\full\path\to\export\xml\directory", logger);
				importer.Import();
			}
			catch (Exception ex)
			{
				logger.Error(ex, "An unexpected error occurred during import");
			}

			return View("/views/ImportUCommerceItems.cshtml");
		}
	}
}