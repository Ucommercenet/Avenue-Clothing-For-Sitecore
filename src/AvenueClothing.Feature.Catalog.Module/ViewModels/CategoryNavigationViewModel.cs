using System.Collections.Generic;

namespace AvenueClothing.Feature.Catalog.Module.ViewModels
{
	public class CategoryNavigationViewModel
	{
		public CategoryNavigationViewModel()
		{
			Categories = new List<CategoryViewModel>();
		}
		public IList<CategoryViewModel> Categories { get; set; } 
	}
}