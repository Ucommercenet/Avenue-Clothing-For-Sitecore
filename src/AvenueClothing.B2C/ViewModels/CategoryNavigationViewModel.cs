using System.Collections.Generic;

namespace AvenueClothing.B2C.ViewModels
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