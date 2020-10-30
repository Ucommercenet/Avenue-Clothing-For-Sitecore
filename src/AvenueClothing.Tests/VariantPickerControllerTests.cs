using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using AvenueClothing.Project.Transaction.Controllers;
using AvenueClothing.Project.Transaction.ViewModels;
using AvenueClothing.Project.Website.Extensions;
using NSubstitute;
using Ucommerce.Api;
using Ucommerce.Infrastructure.Globalization;
using Ucommerce.Pipelines;
using Ucommerce.Pipelines.GetProduct;
using Ucommerce.Search;
using Ucommerce.Search.Definitions;
using Ucommerce.Search.Models;
using Xunit;

namespace AvenueClothing.Tests
{
    public class VariantPickerControllerTests
    {
        private readonly IPipeline<IPipelineArgs<GetProductRequest, GetProductResponse>> _getProductPipeline;
        private readonly ICatalogContext _catalogContext;
        private readonly ICatalogLibrary _catalogLibrary;
        private readonly VariantPickerController _controller;
        private readonly IIndex<Product> _productIndex;
        private readonly ILocalizationContext _localizationContext;

        public VariantPickerControllerTests()
        {
            _getProductPipeline = Substitute.For<IPipeline<IPipelineArgs<GetProductRequest, GetProductResponse>>>();
            _catalogContext = Substitute.For<ICatalogContext>();
            _catalogLibrary = Substitute.For<ICatalogLibrary>();
            _productIndex = Substitute.For<IIndex<Product>>();
            _productIndex.Definition.Returns(new AvenueProductIndexDefinition());
            _localizationContext = Substitute.For<ILocalizationContext>();
            _localizationContext.CurrentCulture.Returns(CultureInfo.GetCultureInfo("en-US"));

            _controller = new VariantPickerController(_getProductPipeline, _catalogContext, _catalogLibrary, _productIndex, _localizationContext);

            _controller.Url = Substitute.For<UrlHelper>();
            _controller.Url.Action(Arg.Any<string>()).Returns("ControllerUrl");
        }

        [Fact]
        public void Rendering_Should_Return_NonEmpty_ViewModel()
        {
            // Arrange
            _catalogContext.CurrentProduct = new Product();
            _catalogContext.CurrentProduct.Sku = "PRD-01";
            _catalogContext.CurrentProduct.ProductDefinition = Guid.NewGuid();
            _catalogContext.CurrentProduct.ProductType = ProductType.ProductFamily;

            var variant = new Product
            {
                Guid = Guid.NewGuid(),
                Name = "Variant 1",
                Sku = "Variant-01"
            };
            variant["DisplayOnSite"] = true;

            _catalogContext.CurrentProduct.Variants = new List<Guid>();
            _catalogContext.CurrentProduct.Variants.Add(variant.Guid);

            var variants = new ResultSet<Product>(new List<Product> {variant}, 1);

            _catalogLibrary.GetVariants(_catalogContext.CurrentProduct).Returns(variants);


            // Act
            var result = _controller.Rendering();

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult != null ? viewResult.Model as VariantPickerRenderingViewModel : null;
            Assert.NotNull(viewResult);
            Assert.NotNull(model);
            Assert.NotEmpty(model.Variants);
            Assert.NotNull(model.GetAvailableCombinationsUrl);
        }


    }
}
