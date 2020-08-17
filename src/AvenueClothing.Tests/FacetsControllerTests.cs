using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AvenueClothing.Project.Catalog.Controllers;
using AvenueClothing.Project.Catalog.ViewModels;
using NSubstitute;
using Ucommerce.Api;
using Ucommerce.Search.Facets;
using Ucommerce.Search.Models;
using Xunit;

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
            _catalogLibrary = Substitute.For<ICatalogLibrary>();
            _controller = new FacetsController(_catalogContext, _catalogLibrary);
        }

        [Fact]
        public void Rendering_Should_Return_Valid_ViewModel_With_Non_Null_Attributes()
        {
            // Arrange
            IList<Facet> facetsForQuerying = new List<Facet>();

            var facetValues = new List<FacetValue>();
            facetValues.Add(new FacetValue
            {
                Value = "122",
                Count = 2
            });
            facetValues.Add(new FacetValue
            {
                Value = "127",
                Count = 0
            });

            facetsForQuerying.Add(new Facet
            {
                Name = "Price",
                DisplayName = "Price",
                FacetValues = facetValues
            });

            _catalogContext.CurrentCategory = Substitute.For<Category>();
            var returnedFacets = new List<Facet>();
            var facetValue = new FacetValue
            {
                Value = "177",
                Count = 4
            };
            var facetValueList = new List<FacetValue>();
            facetValueList.Add(facetValue);

            returnedFacets.Add(new Facet
            {
                Name = "Price",
                DisplayName = "Price",
                FacetValues = facetValueList
            });

            _catalogLibrary.GetFacets(Guid.Empty, new FacetDictionary()).ReturnsForAnyArgs(returnedFacets);

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
            facetValues.Add(new FacetValue
            {
                Value = "122",
                Count = 2
            });
            facetValues.Add(new FacetValue
            {
                Value = "127",
                Count = 0
            });

            facetsForQuerying.Add(new Facet
            {
                Name = "Price",
                DisplayName = "Price",
                FacetValues = facetValues
            });

            _catalogContext.CurrentCategory = Substitute.For<Category>();
            var returnedFacets = new List<Facet>();
            var facetValue = new FacetValue
            {
                Value = "177",
                Count = 4
            };
            var facetValueList = new List<FacetValue>();
            facetValueList.Add(facetValue);

            returnedFacets.Add(new Facet
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
