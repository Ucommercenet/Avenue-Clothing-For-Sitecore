using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AvenueClothing.Feature.Transaction.Module.Controllers;
using AvenueClothing.Feature.Transaction.Module.ViewModels;
using AvenueClothing.Tests.Fakes;
using UCommerce.EntitiesV2;
using Xunit;

namespace AvenueClothing.Tests
{
    public class MiniBasketControllerTests
    {
        [Fact]
        public void MiniBasket_When_Basket_Is_Empty_Should_Return_View_With_Empty_Model()
        {
            //Create
            var fakeTransactionLibraryInternal = new FakeTransactionLibraryInternal();
            var miniBasketController = new MiniBasketController(fakeTransactionLibraryInternal);
            

            //Arrange
            fakeTransactionLibraryInternal.HasBasketFunc = () => false;

            //Act
            var result = miniBasketController.MiniBasket();

            //Assert
            var viewResult = result as ViewResult;
            var model = viewResult.Model as MiniBasketViewModel;
            Assert.NotNull(viewResult);
            Assert.NotNull(model);
            Assert.True(model.IsEmpty);
            Assert.Equal(0, model.NumberOfItems);
            Assert.Null(model.Total);
            Assert.NotEmpty(model.RefreshUrl);

            //var result2 = miniBasketController.Refresh();
        }

        [Fact]
        public void MiniBasket_When_Basket_Is_Not_Empty_Should_Return_View_With_Non_Empty_Model()
        {
            //Create
            var fakeTransactionLibraryInternal = new FakeTransactionLibraryInternal();
            var miniBasketController = new MiniBasketController(fakeTransactionLibraryInternal);

            //Arrange
            fakeTransactionLibraryInternal.HasBasketFunc = () => true;
            fakeTransactionLibraryInternal.GetBasketFunc = () => new Basket(new PurchaseOrder());

            //Act
            var result = miniBasketController.MiniBasket();

            //Assert
            var viewResult = result as ViewResult;
            var model = viewResult.Model as MiniBasketViewModel;
            Assert.NotNull(viewResult);
            Assert.NotNull(model);
            Assert.True(model.IsEmpty);
            Assert.Equal(0, model.NumberOfItems);
            Assert.Null(model.Total);
            Assert.NotEmpty(model.RefreshUrl);

            //var result2 = miniBasketController.Refresh();
        }
    }
}
