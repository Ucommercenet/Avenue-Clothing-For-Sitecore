using System;
using System.Web.Mvc;
using AvenueClothing.Feature.Transaction.Controllers;
using AvenueClothing.Feature.Transaction.ViewModels;
using NSubstitute;
using UCommerce.Catalog;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;
using UCommerce.Pipelines.GetProduct;
using UCommerce.Runtime;
using Xunit;

namespace AvenueClothing.Feature.Tests
{
    public class VariantPickerControllerTests
    {
        private readonly IPipeline<IPipelineArgs<GetProductRequest, GetProductResponse>> _getProductPipeline;
        private readonly ICatalogContext _catalogContext;
        private readonly CatalogLibraryInternal _catalogLibraryInternal;
        private readonly VariantPickerController _controller;

        public VariantPickerControllerTests()
        {
            _getProductPipeline = Substitute.For<IPipeline<IPipelineArgs<GetProductRequest, GetProductResponse>>>();
            _catalogContext = Substitute.For<ICatalogContext>();
            _catalogLibraryInternal = Substitute.For<CatalogLibraryInternal>(null, null, null, null, null, null, null, null, null, null, null);

            _controller = new VariantPickerController(_getProductPipeline, _catalogContext, _catalogLibraryInternal);

            _controller.Url = Substitute.For<UrlHelper>();
            _controller.Url.Action(Arg.Any<string>()).Returns("ControllerUrl");
        }

        [Fact]
        public void Rendering_Should_Return_NonEmpty_ViewModel()
        {
            // Arrange
            _catalogContext.CurrentProduct = new Product();
            _catalogContext.CurrentProduct.Sku = "PRD-01";
            _catalogContext.CurrentProduct.ProductDefinition = Substitute.For<ProductDefinition>();
            _catalogContext.CurrentProduct.ProductDefinition.IsProductFamily().Returns(true);

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
            
            _catalogContext.CurrentProduct.Variants.Add(variant);

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
