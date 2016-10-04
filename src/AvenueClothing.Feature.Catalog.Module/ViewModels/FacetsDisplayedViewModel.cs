using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AvenueClothing.Feature.Catalog.Module.ViewModels
{
	public class FacetsDisplayedViewModel
	{
		public FacetsDisplayedViewModel()
		{
			Facets = new List<FacetViewModel>();
		}
		public IList<FacetViewModel> Facets { get; set; }
	}
}