﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Project.Transaction.Controllers;
using AvenueClothing.Project.Transaction.ViewModels;
using NSubstitute;
using Ucommerce.Api;
using Ucommerce.EntitiesV2;
using Xunit;

namespace AvenueClothing.Tests
{
    public class AddressControllerTests
    {
        private readonly AddressController _controller;
        private readonly ITransactionLibrary _transactionLibraryInternal;
        private readonly List<Country> _countries;

        public AddressControllerTests()
        {
            //Create
            _transactionLibraryInternal = Substitute.For<ITransactionLibrary>();
            _countries = new List<Country>();
            _controller = new AddressController(_transactionLibraryInternal, _countries.AsQueryable());

            _controller.Url = Substitute.For<UrlHelper>();
            _controller.Url.Action(Arg.Any<string>()).Returns("ControllerUrl");
        }

        [Fact]
        public void Rendering_When_Basket_Has_Shipping_And_Billing_Information_Should_Return_View_With_Non_Empty_Model()
        {
            //Arrange
            _countries.Add(new Country());
            _transactionLibraryInternal.GetBasket().Returns(new PurchaseOrder());

            //Act
            var result = _controller.Rendering();

            //Assert
            var viewResult = result as ViewResult;
            var model = viewResult != null ? viewResult.Model as AddressRenderingViewModel : null;
            Assert.NotNull(viewResult);
            Assert.NotNull(model);
            Assert.NotNull(model.BillingAddress);
            Assert.NotNull(model.ShippingAddress);
            Assert.NotEmpty(model.AvailableCountries);
        }

        [Fact]
        public void Save_When_With_Valid_Data_Should_Execute_Basket_Pipeline_And_Return_Json()
        {
            //Arrange
            var viewModel = new AddressSaveViewModel();

            //Act
            var result = _controller.Save(viewModel);

            //Assert
            _transactionLibraryInternal.Received().ExecuteBasketPipeline();
            var jsonResult = result as JsonResult;
            Assert.NotNull(jsonResult);
        }
    }
}