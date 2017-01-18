using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class PaymentPickerControllerTests
    {
        private readonly PaymentPickerController _controller;
        private readonly TransactionLibraryInternal _transactionLibraryInternal;

        public PaymentPickerControllerTests()
        {
            _transactionLibraryInternal = Substitute.For<TransactionLibraryInternal>(null, null, null, null, null, null,
                null, null, null, null, null);

            _controller = new PaymentPickerController(_transactionLibraryInternal);
        }

        [Fact]
        public void Rendering_Returns_View_With_Valid_Model()
        {
            // Arrange
            
            var purchaseOrder = Substitute.For<PurchaseOrder>();
            purchaseOrder.GetShippingAddress(Constants.DefaultShipmentAddressName).Returns(new OrderAddress()
            {
                Country = new Country()
                {
                    Name = "Denmark"
                }
            });

            var paymentMethods = new List<PaymentMethod>();
            paymentMethods.Add(new PaymentMethod()
            {
                Name = "PayPal"
            });

            _transactionLibraryInternal.GetPaymentMethods(
                purchaseOrder.GetShippingAddress(Constants.DefaultShipmentAddressName).Country)
                .Returns(paymentMethods);


            var basket = Substitute.For<Basket>(purchaseOrder);
            _transactionLibraryInternal.GetBasket(false).Returns(basket);

            // Act
            var result = _controller.Rendering();

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult != null ? viewResult.Model as PaymentPickerViewModel : null;

            Assert.NotNull(model);
            Assert.NotNull(viewResult);
            Assert.NotNull(model.SelectedPaymentMethodId);
            Assert.True(model.AvailablePaymentMethods.Count == 1);

            _transactionLibraryInternal.Received().GetPaymentMethods(purchaseOrder.GetShippingAddress(Constants.DefaultShipmentAddressName).Country);
        }

        [Fact]
        public void CreatePayment_And_ExecuteBasketPipeline_Are_Called()
        {
            // Arrange
            var viewModel = new PaymentPickerViewModel
            {
                SelectedPaymentMethodId = 0,
                ShippingCountry = "Denmark"
            };
            viewModel.AvailablePaymentMethods.Add(new SelectListItem
            {
                Selected = true,
                Text = "Text",
                Value = "Paypal"
            });

            // Act
            var result = _controller.CreatePayment(viewModel);

            // Assert
            _transactionLibraryInternal.Received().CreatePayment(viewModel.SelectedPaymentMethodId, -1m, false, true);
            _transactionLibraryInternal.Received().ExecuteBasketPipeline();
        }
    }
}
