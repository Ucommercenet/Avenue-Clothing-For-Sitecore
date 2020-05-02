using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Project.Catalog.ViewModels;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Catalog.Services;
using Ucommerce.Api;
using Ucommerce.Search.Extensions;
using Ucommerce.Search.Facets;

namespace AvenueClothing.Project.Catalog.Controllers
{
    public class FacetsController : BaseController
    {
        private readonly ICatalogContext _catalogContext;
        private readonly ICatalogLibrary _catalogLibrary;

        public FacetsController(ICatalogContext catalogContext, ICatalogLibrary catalogLibrary)
        {
            _catalogContext = catalogContext;
            _catalogLibrary = catalogLibrary;
        }

        public ActionResult Rendering([ModelBinder(typeof(FacetModelBinder))]IList<Facet> facetsForQuerying)
        {

            var category = _catalogContext.CurrentCategory;
            var viewModel = new FacetsRenderingViewModel();

            IList<Facet> facets = _catalogLibrary.GetFacets(category.Guid, facetsForQuerying.ToFacetDictionary());
            if (facets.Any(x => x.FacetValues.Any(y => y.Count > 0)))
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
                    if (value.Count > 0)
                    {
                        var facetVal = new FacetsRenderingViewModel.FacetValue(value.Value, (int) value.Count);
                        facetViewModel.FacetValues.Add(facetVal);
                    }
                }

                facets.Add(facetViewModel);
            }

            return facets;
        }
    }
}
