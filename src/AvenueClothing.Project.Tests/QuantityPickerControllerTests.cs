using System;
using System.Web.Mvc;
using AvenueClothing.Project.Transaction.Controllers;
using AvenueClothing.Project.Transaction.ViewModels;
using NSubstitute;
using UCommerce.EntitiesV2;
using UCommerce.Runtime;
using Xunit;

namespace AvenueClothing.Tests
{
    public class QuantityPickerControllerTests
    {
        private readonly ICatalogContext _catalogContext;
        private readonly QuantityPickerController _controller;

        public QuantityPickerControllerTests()
        {
            _catalogContext = Substitute.For<ICatalogContext>();
            _controller = new QuantityPickerController(_catalogContext);
        }

        [Fact]
        public void Rendering_Returns_View_With_Valid_ViewModel()
        {
            // Arrange
            _catalogContext.CurrentProduct = new Product()
            {
                Guid = new Guid(),
                Name = "Product",
                Sku = "PRD-01"
            };

            // Act
            var result = _controller.Rendering();

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult != null ? viewResult.Model as QuantityPickerRenderingViewModel : null;
            Assert.NotNull(viewResult);
            Assert.NotNull(model);
            Assert.True(model.ProductSku == _catalogContext.CurrentProduct.Sku);
            
        }
    }
}
