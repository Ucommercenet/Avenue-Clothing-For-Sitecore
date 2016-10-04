using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AvenueClothing.Feature.Catalog.Module.ViewModels
{
	public class FacetValueViewModel
	{
		public FacetValueViewModel(string name, int hits)
		{
			FacetValueName = name;
			FacetValueHits = hits;
		}
		public string FacetValueName { get; set; }
		public int FacetValueHits { get; set; }
	}
}