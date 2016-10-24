using System;
using System.Collections.Generic;

namespace AvenueClothing.Feature.Catalog.Module.ViewModels
{
	public class CategoryViewModel
	{
		public CategoryViewModel()
		{
			ProductItemGuids = new List<Guid>();
		}

		public IList<Guid> ProductItemGuids { get; set; }
		public string Name { get; set; }
		public string Url { get; set; }
		public IList<CategoryViewModel> Categories { get; set; }
	}
}