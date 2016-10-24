using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AvenueClothing.Feature.Catalog.Module.ViewModels
{
	public class FacetsRenderingViewModel
	{
		public FacetsRenderingViewModel()
		{
			Facets = new List<Facet>();
		}

		public List<Facet> Facets { get; set; }

        public class Facet
        {
            public Facet()
            {
                FacetValues = new List<FacetValue>();
            }
            public List<FacetValue> FacetValues { get; set; }
            public string Name { get; set; }

            public string DisplayName { get; set; }
        }

        public class FacetValue
        {
            public FacetValue(string name, int hits)
            {
                FacetValueName = name;
                FacetValueHits = hits;
            }
            public string FacetValueName { get; set; }
            public int FacetValueHits { get; set; }
        }
    }
}