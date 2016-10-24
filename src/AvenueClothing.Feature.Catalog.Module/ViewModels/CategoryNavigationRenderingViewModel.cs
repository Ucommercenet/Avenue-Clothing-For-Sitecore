using System;
using System.Collections.Generic;

namespace AvenueClothing.Feature.Catalog.Module.ViewModels
{
	public class CategoryNavigationRenderingViewModel
	{
		public CategoryNavigationRenderingViewModel()
		{
			Categories = new List<Category>();
		}
		public List<Category> Categories { get; set; }

        public class Category
        {
            public Category()
            {
                ProductItemGuids = new List<Guid>();
                Categories = new List<Category>();
            }

            public List<Guid> ProductItemGuids { get; set; }
            public string Name { get; set; }
            public string Url { get; set; }
            public List<Category> Categories { get; set; }
        }
    }
}