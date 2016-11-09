using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Feature.Catalog.ViewModels;
using AvenueClothing.Foundation.MvcExtensions;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;

namespace AvenueClothing.Feature.Catalog.Controllers
{
	public class CatalogImageCaruselController : BaseController
    {
		public ActionResult Rendering()
		{
			var item = Sitecore.Mvc.Presentation.RenderingContext.Current.Rendering.Item;
			var slideIds = Sitecore.Data.ID.ParseArray(item["images"]);
			
			var viewModel = new CatalogImageCaruselRenderingViewModel
			{
				ImageUrls = 
					slideIds.Select(i =>
						GetMediaItemUrl(item.Database.GetItem(i))
						).ToList()
			};

			return View(viewModel);
		}

		public static string GetMediaItemUrl(Item item)
		{
			var mediaUrlOptions = new MediaUrlOptions() { UseItemPath = false, AbsolutePath = true };
			return Sitecore.Resources.Media.MediaManager.GetMediaUrl(item, mediaUrlOptions);
		}
    }
}