using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AvenueClothing.Project.Transaction.Controllers;
using AvenueClothing.Project.Transaction.ViewModels;
using NSubstitute;
using UCommerce.EntitiesV2;
using UCommerce.Transactions;
using Xunit;

namespace AvenueClothing.Tests
{
    public class ShippingPickerControllerTests
    {
        private readonly ShippingPickerController _controller;
        private readonly TransactionLibraryInternal _transactionLibraryInternal;

        public ShippingPickerControllerTests()
        {
            _transactionLibraryInternal = Substitute.For<TransactionLibraryInternal>(null, null, null, null, null, null, null, null, null, null, null);
            _controller = new ShippingPickerController(_transactionLibraryInternal);
        }

        [Fact]
        public void Rendering_When_Shipping_Has_New_Model()
        {
            //Arrange
            _transactionLibraryInternal.GetBasket().Returns(new Basket(new PurchaseOrder()));
            //Act
            var result = _controller.Rendering();
            //Assert
            var viewResult = result as ViewResult;
            var model = viewResult != null ? viewResult.Model as ShippingPickerViewModel : null;
            Assert.NotNull(viewResult);
            Assert.NotNull(model);
            Assert.NotEmpty(model.AvailableShippingMethods);
            Assert.NotNull(model.SelectedShippingMethodId);
            Assert.NotNull(model.ShippingCountry);
        }
    }
}
