using System.Collections.Generic;

namespace AvenueClothing.Project.Catalog.ViewModels
{
	public class CatalogImageCaruselRenderingViewModel
	{
		public IList<string> ImageUrls { get; set; }

		public CatalogImageCaruselRenderingViewModel()
		{
			ImageUrls = new List<string>();
		}
	}
}