using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AvenueClothing.Feature.Catalog.ViewModels
{
	public class CatalogImageCaruselRenderingViewModel
	{
		public List<string> ImageUrls { get; set; }

		public CatalogImageCaruselRenderingViewModel()
		{
			ImageUrls = new List<string>();
		}
	}
}