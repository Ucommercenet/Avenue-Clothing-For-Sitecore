using System.Collections.Generic;
using System.Web.Mvc;
using AvenueClothing.Project.Transaction.Controllers;
using AvenueClothing.Project.Transaction.Services.Impl;
using AvenueClothing.Project.Transaction.ViewModels;
using NSubstitute;
using Ucommerce.Api;
using Ucommerce.EntitiesV2;
using Ucommerce.Search.Slugs;
using Xunit;

namespace AvenueClothing.Tests
{
    public class BasketControllerTests
    {
        private readonly BasketController _controller;
        private readonly ITransactionLibrary _transactionLibraryInternal;
        private readonly MiniBasketService _miniBasketService;
        private readonly IUrlService _urlService;
        private readonly ICatalogLibrary _catalogLibrary;
        private readonly ICatalogContext _catalogContext;

        public BasketControllerTests()
        {
            // Create
            _transactionLibraryInternal = Substitute.For<ITransactionLibrary>();
            _miniBasketService = Substitute.For<MiniBasketService>(_transactionLibraryInternal);
            _urlService = Substitute.For<IUrlService>();
            _catalogContext = Substitute.For<ICatalogContext>();
            _catalogLibrary = Substitute.For<ICatalogLibrary>();

            _controller = new BasketController(_transactionLibraryInternal, _miniBasketService, _urlService, _catalogContext, _catalogLibrary);

            _controller.Url = Substitute.For<UrlHelper>();
            _controller.Url.Action(Arg.Any<string>()).Returns("anything");
        }

        [Fact]
        public void Rendering_When_BasketModel_IsNotNull_Should_Return_View_With_Non_Empty_Model()
        {
            // Arrange
            _transactionLibraryInternal.GetBasket().Returns(new PurchaseOrder
            {
                BillingCurrency = new Currency
                {
                    ISOCode = "USD"
                }
            });
            _transactionLibraryInternal.HasBasket().Returns(true);


            // Act
            var result = _controller.Rendering();

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult != null ? viewResult.Model as BasketRenderingViewModel : null;

            Assert.NotNull(viewResult);
            Assert.NotNull(model);
            Assert.NotEmpty(model.RefreshUrl);
            Assert.NotEmpty(model.RemoveOrderlineUrl);

            Assert.NotNull(model.OrderTotal);
            Assert.NotNull(model.DiscountTotal);
            Assert.NotNull(model.TaxTotal);
            Assert.NotNull(model.SubTotal);
        }

        [Fact]
        public void RemoveOrderline_UpdatesLineItemByOrderLineId_Should_Return_Json()
        {
            // Arrange
            var orderlineToRemoveId = 1;

            // Act
            var result = _controller.RemoveOrderline(orderlineToRemoveId);

            // Assert
            _transactionLibraryInternal.Received().UpdateLineItemByOrderLineId(orderlineToRemoveId, 0);
            var jsonResult = result as JsonResult;
            Assert.NotNull(jsonResult);
        }

        [Fact]
        public void UpdateOrderline_Removes_Orderline_If_Quantity_Is_Negative_Or_Zero_And_Json_Result_IsNotNull()
        {
            //Arrange
            _transactionLibraryInternal.GetBasket().Returns(new PurchaseOrder
            {
                BillingCurrency = new Currency
                {
                    ISOCode = "USD"
                }
            });
            BasketUpdateBasket model = new BasketUpdateBasket();
            model.RefreshBasket = new List<BasketUpdateBasket.UpdateOrderLine>();
            model.RefreshBasket.Add(new BasketUpdateBasket.UpdateOrderLine()
            {
                OrderLineId = 0,
                OrderLineQty = 2
            });
            model.RefreshBasket.Add(new BasketUpdateBasket.UpdateOrderLine()
            {
                OrderLineId = 1,
                OrderLineQty = 1
            });
            model.RefreshBasket.Add(new BasketUpdateBasket.UpdateOrderLine()
            {
                OrderLineId = 2,
                OrderLineQty = -5
            });

            //Act
            var result = _controller.UpdateBasket(model);

            // Assert
            _transactionLibraryInternal.Received().UpdateLineItemByOrderLineId(0, 2);
            _transactionLibraryInternal.Received().UpdateLineItemByOrderLineId(1, 1);
            _transactionLibraryInternal.Received().UpdateLineItemByOrderLineId(2, 0);
            _transactionLibraryInternal.Received().ExecuteBasketPipeline();

            var jsonResult = result as JsonResult;
            Assert.NotNull(jsonResult);
        }
    }
}
