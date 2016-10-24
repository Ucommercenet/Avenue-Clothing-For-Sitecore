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
		private readonly ICatalogContext _catalogContext;

		public FacetsController(ICatalogContext catalogContext)
		{
			_catalogContext = catalogContext;
		}

		public ActionResult Rendering()
		{
			var category = _catalogContext.CurrentCategory;
			var viewModel = new FacetsRenderingViewModel();
			IList<Facet> facetsForQuerying = System.Web.HttpContext.Current.Request.QueryString.ToFacets();
			
			IList<Facet> facets = SearchLibrary.GetFacetsFor(category, facetsForQuerying);
			if (facets.Any(x => x.FacetValues.Any(y => y.Hits > 0)))
			{
				viewModel.Facets = MapFacets(facets);
			}

			return View(viewModel);
		}

		private List<FacetsRenderingViewModel.Facet> MapFacets(IList<Facet> facetsInCategory)
		{
			var facets = new List<FacetsRenderingViewModel.Facet>();

			foreach (var facet in facetsInCategory)
			{
			    var facetViewModel = new FacetsRenderingViewModel.Facet
			    {
			        Name = facet.Name,
			        DisplayName = facet.DisplayName
			    };

			    if (!facet.FacetValues.Any())
				{
					continue;
				}

				foreach (var value in facet.FacetValues)
				{
					if (value.Hits > 0)
					{
                        var facetVal = new FacetsRenderingViewModel.FacetValue(value.Value, value.Hits);
						facetViewModel.FacetValues.Add(facetVal);
					}
				}

				facets.Add(facetViewModel);
			}

			return facets;
		}
	}

}
