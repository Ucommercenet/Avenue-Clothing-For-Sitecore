using System;
using System.Web.Mvc;
using AvenueClothing.Project.Catalog.Controllers;
using AvenueClothing.Project.Catalog.ViewModels;
using NSubstitute;
using Ucommerce.Api;
using Ucommerce.Catalog;
using Ucommerce.Search;
using Xunit;

namespace AvenueClothing.Tests
{
    public class ProductPriceControllerTests
    {
        private readonly ICatalogContext _catalogContext;
        private readonly ProductPriceController _controller;
        private readonly IIndex<Ucommerce.Search.Models.Product> _productIndex;
        private readonly IProductPriceCalculationService _productPriceCalculationService;

        public ProductPriceControllerTests()
        {
            _catalogContext = Substitute.For<ICatalogContext>();
            _productIndex = Substitute.For<IIndex<Ucommerce.Search.Models.Product>>();
            _productPriceCalculationService = Substitute.For<IProductPriceCalculationService>();

            _controller = new ProductPriceController(_catalogContext, _productIndex, _productPriceCalculationService);

            _controller.Url = Substitute.For<UrlHelper>();
            _controller.Url.Action(Arg.Any<string>()).Returns("ControllerUrl");
        }

        [Fact]
        public void Rendering_Should_Return_Valid_ViewModel_With_NotNull_Attributes()
        {
            // Arrange
            _catalogContext.CurrentProduct = new Ucommerce.Search.Models.Product
            {
                Sku = "CRNT_PRD",
                Guid = new Guid()
            };
            _catalogContext.CurrentCategory = new Ucommerce.Search.Models.Category
            {
                Guid = new Guid(),
                Name = "Category"
            };
            _catalogContext.CurrentCatalog = new Ucommerce.Search.Models.ProductCatalog
            {
                Guid = new Guid(),
                Name = "Catalog"
            };

            // Act
            var result = _controller.Rendering();

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult != null ? viewResult.Model as ProductPriceRenderingViewModel : null;

            Assert.NotNull(model);
            Assert.NotNull(viewResult);
            Assert.NotNull(model.Sku);
            Assert.NotNull(model.CalculatePriceUrl);
            Assert.NotNull(model.CalculateVariantPriceUrl);
            Assert.NotNull(model.ProductGuid);
            Assert.NotNull(model.CatalogGuid);
            Assert.NotNull(model.CategoryGuid);
        }

        [Fact]
        public void Rendering_Should_Return_ViewModel_With_Empty_CategoryGuid_If_CurrentCategory_IsNull()
        {
            // Arrange
            _catalogContext.CurrentProduct = new Ucommerce.Search.Models.Product
            {
                Sku = "CRNT_PRD",
                Guid = new Guid()
            };
            _catalogContext.CurrentCategory = null;
            _catalogContext.CurrentCatalog = new Ucommerce.Search.Models.ProductCatalog
            {
                Guid = new Guid(),
                Name = "Catalog"
            };

            // Act
            var result = _controller.Rendering();

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult != null ? viewResult.Model as ProductPriceRenderingViewModel : null;

            Assert.NotNull(model);
            Assert.NotNull(viewResult);
            Assert.Equal(Guid.Empty, model.CategoryGuid);
        }

    }
}
