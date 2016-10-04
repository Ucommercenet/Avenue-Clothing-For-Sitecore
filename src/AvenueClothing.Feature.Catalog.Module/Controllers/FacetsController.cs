using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Feature.Catalog.Module.Extensions;
using AvenueClothing.Feature.Catalog.Module.ViewModels;
using UCommerce.Api;
using UCommerce.Content;
using UCommerce.EntitiesV2;
using UCommerce.Extensions;
using UCommerce.Infrastructure;
using UCommerce.Runtime;
using UCommerce.Search.Facets;
using UCommerce.Search;

namespace AvenueClothing.Feature.Catalog.Module.Controllers
{
	public class FacetsController : Controller
	{
		public ActionResult Facets()
		{
			var category = SiteContext.Current.CatalogContext.CurrentCategory;
			var facetValueOutputModel = new FacetsDisplayedViewModel();
			IList<Facet> facetsForQuerying = System.Web.HttpContext.Current.Request.QueryString.ToFacets();
			
			IList<Facet> facets = SearchLibrary.GetFacetsFor(category, facetsForQuerying);
			if (facets.Any(x => x.FacetValues.Any(y => y.Hits > 0)))
			{
				facetValueOutputModel.Facets = MapFacets(facets);
			}

			return View("/views/Facets.cshtml", facetValueOutputModel);
		}

		private IList<FacetViewModel> MapFacets(IList<Facet> facetsInCategory)
		{
			IList<FacetViewModel> facets = new List<FacetViewModel>();

			foreach (var facet in facetsInCategory)
			{
				var facetViewModel = new FacetViewModel();
				facetViewModel.Name = facet.Name;
				facetViewModel.DisplayName = facet.DisplayName;

				if (!facet.FacetValues.Any())
				{
					continue;
				}

				foreach (var value in facet.FacetValues)
				{
					if (value.Hits > 0)
					{
						FacetValueViewModel facetVal = new FacetValueViewModel(value.Value, value.Hits);
						facetViewModel.FacetValues.Add(facetVal);
					}
				}

				facets.Add(facetViewModel);
			}

			return facets;
		}
	}

}
