using System.Collections.Generic;
using System.Web.Mvc;
using NSubstitute;
using AvenueClothing.Project.Catalog.Controllers;
using AvenueClothing.Project.Catalog.ViewModels;
using Ucommerce.Api;
using Ucommerce.EntitiesV2;
using Ucommerce.Search.Facets;
using Xunit;
using Category = Ucommerce.Search.Models.Category;

namespace AvenueClothing.Tests
{
    public class FacetsControllerTests
    {
        private readonly ICatalogContext _catalogContext;
        private readonly FacetsController _controller;
        private readonly ICatalogLibrary _catalogLibrary;

        public FacetsControllerTests()
        {
            _catalogContext = Substitute.For<ICatalogContext>();
            _controller = new FacetsController(_catalogContext, _catalogLibrary);
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
                Count = 2
            });
            facetValues.Add(new FacetValue()
            {
                Value = "127",
                Count = 0
            });

            facetsForQuerying.Add(new Facet()
            {
                Name = "Price",
                DisplayName = "Price",
                FacetValues = facetValues
            });

            _catalogContext.CurrentCategory = Substitute.For<Ucommerce.Search.Models.Category>();
            var returnedFacets = new List<Facet>();
            var facetValue = new FacetValue()
            {
                Value = "177",
                Count = 4
            };
            var facetValueList = new List<FacetValue>();
            facetValueList.Add(facetValue);

            returnedFacets.Add(new Facet()
            {
                Name = "Price",
                DisplayName = "Price",
                FacetValues = facetValueList
            });

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
                Count = 2
            });
            facetValues.Add(new FacetValue()
            {
                Value = "127",
                Count = 0
            });

            facetsForQuerying.Add(new Facet()
            {
                Name = "Price",
                DisplayName = "Price",
                FacetValues = facetValues
            });

            _catalogContext.CurrentCategory = Substitute.For<Category>();
            var returnedFacets = new List<Facet>();
            var facetValue = new FacetValue()
            {
                Value = "177",
                Count = 4
            };
            var facetValueList = new List<FacetValue>();
            facetValueList.Add(facetValue);

            returnedFacets.Add(new Facet()
            {
                Name = "Price",
                DisplayName = "Price",
                FacetValues = facetValueList
            });

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
