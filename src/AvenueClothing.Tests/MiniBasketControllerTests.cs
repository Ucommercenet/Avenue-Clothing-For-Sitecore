using System.Web.Mvc;
using AvenueClothing.Project.Transaction.Controllers;
using AvenueClothing.Project.Transaction.Services.Impl;
using AvenueClothing.Project.Transaction.ViewModels;
using NSubstitute;
using Ucommerce.Api;
using Ucommerce.EntitiesV2;
using Ucommerce.Transactions;
using Xunit;

namespace AvenueClothing.Tests
{
    public class MiniBasketControllerTests
    {
        private readonly MiniBasketController _controller;
        private readonly ITransactionLibrary _transactionLibraryInternal;
	    private MiniBasketService _miniBasketService;

	    public MiniBasketControllerTests()
        {
            //Create
            _transactionLibraryInternal = Substitute.For<ITransactionLibrary>();
			_miniBasketService = Substitute.For<MiniBasketService>(_transactionLibraryInternal);

			_controller = new MiniBasketController(_transactionLibraryInternal, _miniBasketService);

            _controller.Url = Substitute.For<UrlHelper>();
            _controller.Url.Action(Arg.Any<string>()).Returns("anything");
        }

        [Fact]
        public void Rendering_When_Basket_Is_Empty_Should_Return_View_With_Empty_Model()
        {
            //Arrange
            _transactionLibraryInternal.HasBasket().Returns(false);

            //Act
            var result = _controller.Rendering();

            //Assert
            var viewResult = result as ViewResult;
            var model = viewResult != null ? viewResult.Model as MiniBasketRenderingViewModel : null;
            Assert.NotNull(viewResult);
            Assert.NotNull(model);
            Assert.True(model.IsEmpty);
            Assert.Equal(0, model.NumberOfItems);
            Assert.Null(model.Total);
            Assert.NotEmpty(model.RefreshUrl);
        }

        [Fact]
        public void Rendering_When_Basket_Is_Not_Empty_Should_Return_View_With_Non_Empty_Model()
        {
            //Arrange
            _transactionLibraryInternal.HasBasket().Returns(true);
            _transactionLibraryInternal.GetBasket().Returns(new PurchaseOrder
            {
                BillingCurrency = new Currency()
                {
                    ISOCode = "EUR"
                }
            });

            //Act
            var result = _controller.Rendering();

            //Assert
            var viewResult = result as ViewResult;
            var model = viewResult != null ? viewResult.Model as MiniBasketRenderingViewModel : null;
            Assert.NotNull(viewResult);
            Assert.NotNull(model);
            Assert.True(model.IsEmpty);
            Assert.Equal(0, model.NumberOfItems);
            Assert.Equal(0, model.Total.Value);
            Assert.NotEmpty(model.RefreshUrl);
        }

        [Fact]
        public void Refresh_When_Basket_Is_Empty_Should_Return_View_With_Empty_Model()
        {
            //Arrange
            _transactionLibraryInternal.HasBasket().Returns(false);

            //Act
            var result = _controller.Refresh();

            //Assert
            var jsonResult = result as JsonResult;
            var model = jsonResult != null ? jsonResult.Data as MiniBasketRefreshViewModel : null;
            Assert.NotNull(jsonResult);
            Assert.NotNull(model);
            Assert.True(model.IsEmpty);
            Assert.Null(model.NumberOfItems);
            Assert.Null(model.Total);
        }

        [Fact]
        public void Refresh_When_Basket_Is_Not_Empty_Should_Return_View_With_Non_Empty_Model()
        {
            //Arrange
            _transactionLibraryInternal.HasBasket().Returns(true);
            _transactionLibraryInternal.GetBasket().Returns(new PurchaseOrder
            {
                BillingCurrency = new Currency
                {
                    ISOCode = "EUR"
                }
            });

            //Act
            var result = _controller.Refresh();

            //Assert
            var jsonResult = result as JsonResult;
            var model = jsonResult != null ? jsonResult.Data as MiniBasketRefreshViewModel : null;
            Assert.NotNull(jsonResult);
            Assert.NotNull(model);
            Assert.True(model.IsEmpty);
            Assert.NotNull(model.NumberOfItems);
            Assert.NotNull(model.Total);
        }
    }
}
