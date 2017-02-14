using System.Collections.Generic;
using System.Web.Mvc;
using AvenueClothing.Feature.Catalog.Controllers;
using NSubstitute;
using UCommerce.Runtime;
using UCommerce.Search;
using AvenueClothing.Feature.Catalog.ViewModels;
using UCommerce.EntitiesV2;
using UCommerce.Search.Facets;
using Xunit;

namespace AvenueClothing.Feature.Tests
{
    public class FacetsControllerTests
    {
        private readonly ICatalogContext _catalogContext;
        private readonly SearchLibraryInternal _searchLibraryInternal;
        private readonly FacetsController _controller;
        private readonly IFacetedSearch _facetedSearch;

        public FacetsControllerTests()
        {
            _catalogContext = Substitute.For<ICatalogContext>();
            _searchLibraryInternal = Substitute.For<SearchLibraryInternal>(_facetedSearch);
            _controller = new FacetsController(_catalogContext, _searchLibraryInternal);
        }

        [Fact]
        public void Rendering_Should_Return_Valid_ViewModel_With_Non_Null_Attributes()
        {
            // Arrange
            IList<Facet> facetsForQuerying = new List<Facet>();

            var facetValues = new List<FacetValue>();
            facetValues.Add(new FacetValue()
            {
                Value = "122",
                Hits = 2
            });
            facetValues.Add(new FacetValue()
            {
                Value = "127",
                Hits = 0
            });

            facetsForQuerying.Add(new Facet()
            {
                Name = "Price",
                DisplayName = "Price",
                CultureCode = "en-GB",
                FacetValues = facetValues
            });

            _catalogContext.CurrentCategory = Substitute.For<Category>();
            var returnedFacets = new List<Facet>();
            var facetValue = new FacetValue()
            {
                Value = "177",
                Hits = 4
            };
            var facetValueList = new List<FacetValue>();
            facetValueList.Add(facetValue);

            returnedFacets.Add(new Facet()
            {
                Name = "Price",
                DisplayName = "Price",
                CultureCode = "en-GB",
                FacetValues = facetValueList
            });

            _searchLibraryInternal.GetFacetsFor(_catalogContext.CurrentCategory, facetsForQuerying).Returns(returnedFacets);

            // Act
            var result = _controller.Rendering(facetsForQuerying);

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult != null ? viewResult.Model as FacetsRenderingViewModel : null;
            Assert.NotNull(viewResult);
            Assert.NotNull(model);
            Assert.NotEmpty(model.Facets);
        }

        [Fact]
        public void Rendering_Should_Return_Empty_FacetList_If_SearchLibrary_Returns_None()
        {
            // Arrange
            IList<Facet> facetsForQuerying = new List<Facet>();

            var facetValues = new List<FacetValue>();
            facetValues.Add(new FacetValue()
            {
                Value = "122",
                Hits = 2
            });
            facetValues.Add(new FacetValue()
            {
                Value = "127",
                Hits = 0
            });

            facetsForQuerying.Add(new Facet()
            {
                Name = "Price",
                DisplayName = "Price",
                CultureCode = "en-GB",
                FacetValues = facetValues
            });

            _catalogContext.CurrentCategory = Substitute.For<Category>();
            var returnedFacets = new List<Facet>();
            var facetValue = new FacetValue()
            {
                Value = "177",
                Hits = 4
            };
            var facetValueList = new List<FacetValue>();
            facetValueList.Add(facetValue);

            returnedFacets.Add(new Facet()
            {
                Name = "Price",
                DisplayName = "Price",
                CultureCode = "en-GB",
                FacetValues = facetValueList
            });

            _searchLibraryInternal.GetFacetsFor(_catalogContext.CurrentCategory, facetsForQuerying).Returns(new List<Facet>());

            // Act
            var result = _controller.Rendering(facetsForQuerying);

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult != null ? viewResult.Model as FacetsRenderingViewModel : null;
            Assert.NotNull(viewResult);
            Assert.NotNull(model);
            Assert.Empty(model.Facets);
        }
    }
}
