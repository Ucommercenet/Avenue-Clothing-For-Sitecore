using System.Web.Mvc;
using AvenueClothing.Project.Transaction.Controllers;
using AvenueClothing.Project.Transaction.ViewModels;
using NSubstitute;
using UCommerce;
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
            var purchaseOrder = new PurchaseOrder();
            var orderaddress = new OrderAddress { AddressName = Constants.DefaultShipmentAddressName, Country = new Country() };
            purchaseOrder.OrderAddresses.Add(orderaddress);
            purchaseOrder.OrderStatus = new OrderStatus();
            _transactionLibraryInternal.GetBasket(false).Returns(new Basket(purchaseOrder));

            //Act
            var result = _controller.Rendering();

            //Assert
            var viewResult = result as ViewResult;
            var model = viewResult != null ? viewResult.Model as ShippingPickerViewModel : null;
            Assert.NotNull(viewResult);
            Assert.NotNull(model);
            Assert.Equal(-1, model.SelectedShippingMethodId);
        }

        [Fact]
        public void Save_Shipping_Method_Should_Execute_Basket_Pipeline_And_Create_Shippiment()
        {
            //Arrange
            var shipping = new ShippingPickerViewModel();

            //Act
            var result = _controller.CreateShipment(shipping);
       
            //Assert
            Assert.NotNull(shipping.SelectedShippingMethodId);
            Assert.NotNull(result);

            _transactionLibraryInternal.Received().ExecuteBasketPipeline();
            _transactionLibraryInternal.Received().CreateShipment(shipping.SelectedShippingMethodId, Constants.DefaultShipmentAddressName, true);

      }
    }
}
