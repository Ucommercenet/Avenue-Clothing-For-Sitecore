using System;
using System.Web.Mvc;
using AvenueClothing.Project.Transaction.Controllers;
using AvenueClothing.Project.Transaction.ViewModels;
using NSubstitute;
using Ucommerce.Api;
using Ucommerce.EntitiesV2;
using Ucommerce.Pipelines;
using Ucommerce.Pipelines.GetProduct;
using Ucommerce.Search;
using Ucommerce.Search.Models;
using Xunit;
using Product = Ucommerce.EntitiesV2.Product;

namespace AvenueClothing.Tests
{
    public class VariantPickerControllerTests
    {
        private readonly IPipeline<IPipelineArgs<GetProductRequest, GetProductResponse>> _getProductPipeline;
        private readonly ICatalogContext _catalogContext;
        private readonly ICatalogLibrary _catalogLibrary;
        private readonly VariantPickerController _controller;
        private readonly IIndex<Ucommerce.Search.Models.Product> _productIndex;

        public VariantPickerControllerTests()
        {
            _getProductPipeline = Substitute.For<IPipeline<IPipelineArgs<GetProductRequest, GetProductResponse>>>();
            _catalogContext = Substitute.For<ICatalogContext>();
            _catalogLibrary = Substitute.For<ICatalogLibrary>(null, null, null, null, null, null, null, null, null, null, null);
            _productIndex = Substitute.For<IIndex<Ucommerce.Search.Models.Product>>();

            _controller = new VariantPickerController(_getProductPipeline, _catalogContext, _catalogLibrary, _productIndex);

            _controller.Url = Substitute.For<UrlHelper>();
            _controller.Url.Action(Arg.Any<string>()).Returns("ControllerUrl");
        }

        [Fact]
        public void Rendering_Should_Return_NonEmpty_ViewModel()
        {
            // Arrange
            _catalogContext.CurrentProduct = new Ucommerce.Search.Models.Product();
            _catalogContext.CurrentProduct.Sku = "PRD-01";
            _catalogContext.CurrentProduct.ProductDefinition = Guid.NewGuid();
            _catalogContext.CurrentProduct.ProductType = ProductType.ProductFamily;

            var variant = new Product
            {
                Guid = new Guid(),
                Name = "Variant 1",
                Sku = "Variant-01"
            };

            variant.ProductProperties.Add(new ProductProperty()
            {
                Value = "DisplayOnSite",
                ProductDefinitionField = new ProductDefinitionField()
                {
                    DisplayOnSite = true
                }
            });

            _catalogContext.CurrentProduct.Variants.Add(variant.Guid);

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
