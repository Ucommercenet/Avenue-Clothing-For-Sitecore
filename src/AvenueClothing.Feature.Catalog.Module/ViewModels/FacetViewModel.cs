using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AvenueClothing.Feature.Catalog.Module.ViewModels
{
	public class FacetViewModel
	{
		public FacetViewModel()
		{
			FacetValues = new List<FacetValueViewModel>();
		}
		public IList<FacetValueViewModel> FacetValues { get; set; }
		public string Name { get; set; }

		public string DisplayName { get; set; }
	}
}