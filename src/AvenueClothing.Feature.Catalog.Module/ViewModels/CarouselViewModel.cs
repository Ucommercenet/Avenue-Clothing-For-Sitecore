using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AvenueClothing.Feature.Catalog.Module.ViewModels
{
	public class CarouselViewModel
	{
		public IList<string> ImageUrls { get; set; }

		public CarouselViewModel()
		{
			ImageUrls = new List<string>();
		}
	}
}