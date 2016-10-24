using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AvenueClothing.Feature.Transaction.Module.Controllers;
using AvenueClothing.Feature.Transaction.Module.ViewModels;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.Transactions;
using Xunit;

namespace AvenueClothing.Tests
{
    public class AddressControllerTests
    {
        private readonly AddressController _controller;
        private readonly TransactionLibraryInternal _transactionLibraryInternal;
        private readonly List<Country> _countries;

        public AddressControllerTests()
        {
            //Create
            _transactionLibraryInternal = Substitute.For<TransactionLibraryInternal>(null, null, null, null, null, null, null, null, null, null, null);
            _countries = new List<Country>();
            _controller = new AddressController(_transactionLibraryInternal, _countries.AsQueryable());
        }

        [Fact]
        public void Rendering_When_Basket_Has_Shipping_And_Billing_Information_Should_Return_View_With_Non_Empty_Model()
        {
            //Arrange
            _countries.Add(new Country());
            _transactionLibraryInternal.GetBasket().Returns(new Basket(new PurchaseOrder()));

            //Act
            var result = _controller.Rendering();

            //Assert
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as AddressRenderingViewModel;
            Assert.NotNull(viewResult);
            Assert.NotNull(model);
            Assert.NotNull(model.BillingAddress);
            Assert.NotNull(model.ShippingAddress);
            Assert.NotEmpty(model.AvailableCountries);
        }

        [Fact]
        public void Save_When_With_Valid_Data_Should_Execute_Basket_Pipeline_And_Return_Http_Ok()
        {
            //Arrange
            var viewModel = new AddressSaveViewModel();

            //Act
            var result = _controller.Save(viewModel);

            //Assert
            _transactionLibraryInternal.Received().ExecuteBasketPipeline();
            var httpStatusResult = result as HttpStatusCodeResult;
            Assert.NotNull(httpStatusResult);
            Assert.Equal((int)HttpStatusCode.OK, httpStatusResult.StatusCode);
        }
    }
}