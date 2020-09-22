using System;
using System.Web.Mvc;
using AvenueClothing.Project.Transaction.Controllers;
using AvenueClothing.Project.Transaction.Services.Impl;
using AvenueClothing.Project.Transaction.ViewModels;
using NSubstitute;
using Ucommerce.Api;
using Xunit;
using Product = Ucommerce.Search.Models.Product;

namespace AvenueClothing.Tests
{
    public class AddToBasketControllerTests
    {
        private readonly AddToBasketButtonController _controller;
        private readonly ITransactionLibrary _transactionLibraryInternal;
        private readonly ICatalogContext _catalogContext;
	    private MiniBasketService _miniBasketService;

	    public AddToBasketControllerTests()
        {
            //Create
            _transactionLibraryInternal = Substitute.For<ITransactionLibrary>();
            _catalogContext = Substitute.For<ICatalogContext>();
			_miniBasketService = Substitute.For<MiniBasketService>(_transactionLibraryInternal);

			_controller = new AddToBasketButtonController(_transactionLibraryInternal, _catalogContext, _miniBasketService);

            _controller.Url = Substitute.For<UrlHelper>();
            _controller.Url.Action(Arg.Any<string>()).Returns("anything");
        }

        [Fact]
        public void Rendering_When_Current_Product_Exists_Should_Return_View_With_Non_Empty_Model()
        {
            //Arrange
            var product = new Product
            {
                Sku = "testsku",
                ProductDefinition = Guid.NewGuid()
            };
            _catalogContext.CurrentProduct.Returns(product);

            //Act
            var result = _controller.Rendering();

            //Assert
            var viewResult = result as ViewResult;
            var model = viewResult != null ? viewResult.Model as AddToBasketButtonRenderingViewModel : null;
            Assert.NotNull(viewResult);
            Assert.NotNull(model);
            Assert.NotEmpty(model.AddToBasketUrl);
            Assert.NotEmpty(model.BasketUrl);
            Assert.True(model.ConfirmationMessageTimeoutInMillisecs > 0);
            Assert.NotEmpty(model.ConfirmationMessageClientId);
            Assert.NotEmpty(model.ProductSku);
            Assert.False(model.IsProductFamily);
        }

        [Fact]
        public void AddToBasket_When_Data_Is_Valid_Should_Return_Json()
        {
            //Arrange
            var viewModel = new AddToBasketButtonAddToBasketViewModel
            {
                Quantity = 1,
                ProductSku = "testsku123",
                VariantSku = "variantsku123"
            };

            //Act
            var result = _controller.AddToBasket(viewModel);

            //Assert
            _transactionLibraryInternal.Received().AddToBasket(viewModel.Quantity, viewModel.ProductSku, viewModel.VariantSku);
            var jsonResult = result as JsonResult;
            Assert.NotNull(jsonResult);
        }
    }
}