using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;
using AvenueClothing.Project.Catalog.Controllers;
using AvenueClothing.Project.Catalog.ViewModels;
using NSubstitute;
using UCommerce.Catalog;
using UCommerce.EntitiesV2;
using UCommerce.Runtime;
using Xunit;

namespace AvenueClothing.Tests
{
    public class ProductPriceControllerTests
    {
        private readonly IRepository<Product> _productRepository;
        private readonly CatalogLibraryInternal _catalogLibraryInternal;
        private readonly ICatalogContext _catalogContext;
        private readonly ProductPriceController _controller;

        public ProductPriceControllerTests()
        {
            _productRepository = Substitute.For<IRepository<Product>>();
            _catalogLibraryInternal = Substitute.For<CatalogLibraryInternal>(null, null, null, null, null, null, null, null, null, null, null);

            _catalogContext = Substitute.For<ICatalogContext>();

            _controller = new ProductPriceController(_productRepository, _catalogLibraryInternal, _catalogContext);

            _controller.Url = Substitute.For<UrlHelper>();
            _controller.Url.Action(Arg.Any<string>()).Returns("ControllerUrl");
        }

        [Fact]
        public void Rendering_Should_Return_Valid_ViewModel_With_NotNull_Attributes()
        {
            // Arrange
            _catalogContext.CurrentProduct = new Product()
            {
                Sku = "CRNT_PRD",
                Guid = new Guid()
            };
            _catalogContext.CurrentCategory = new Category()
            {
                Guid = new Guid(),
                Name = "Category"
            };
            _catalogContext.CurrentCatalog = new ProductCatalog()
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
            Assert.NotNull(model.ProductId);
            Assert.NotNull(model.CatalogGuid);
            Assert.NotNull(model.CategoryGuid);
        }

        [Fact]
        public void Rendering_Should_Return_ViewModel_With_Empty_CategoryGuid_If_CurrentCategory_IsNull()
        {
            // Arrange
            _catalogContext.CurrentProduct = new Product()
            {
                Sku = "CRNT_PRD",
                Guid = new Guid()
            };
            _catalogContext.CurrentCategory = null;
            _catalogContext.CurrentCatalog = new ProductCatalog()
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
