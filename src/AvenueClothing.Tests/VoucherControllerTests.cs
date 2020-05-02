using System.Web.Mvc;
using AvenueClothing.Project.Transaction.Controllers;
using AvenueClothing.Project.Transaction.ViewModels;
using NSubstitute;
using Ucommerce.Api;
using Xunit;

namespace AvenueClothing.Tests
{
    public class VoucherControllerTests
    {
        private readonly IMarketingLibrary _marketingLibrary;
        private readonly ITransactionLibrary _transactionLibrary;
        private readonly VoucherController _controller;

        public VoucherControllerTests()
        {
            _transactionLibrary = Substitute.For<ITransactionLibrary>(null, null, null, null, null, null,
                 null, null, null, null, null);
            _marketingLibrary = Substitute.For<IMarketingLibrary>(null, null, null);

            _controller = new VoucherController(_transactionLibrary, _marketingLibrary);

            _controller.Url = Substitute.For<UrlHelper>();
            _controller.Url.Action(Arg.Any<string>()).Returns("ControllerUrl");
        }

        [Fact]
        public void Rendering_Should_Return_View_With_NotNull_Model()
        {
            // Arrange

            // Act
            var result = _controller.Rendering();

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult != null ? viewResult.Model as VoucherViewModel : null;

            Assert.NotNull(viewResult);
            Assert.NotNull(model);
            Assert.NotNull(model.VoucherUrl);
            Assert.NotNull(model.InputClassSelector);
            Assert.NotNull(model.ButtonClassSelector);
        }

        [Fact]
        public void AddVoucher_Will_Return_JSON_With_Voucher_String_And_Success_Boolean()
        {
            // Arrange
            string voucher = "VoucherCode2000";

            // Act
            var result = _controller.AddVoucher(voucher);

            // Assert
            var viewResult = result as JsonResult;
            Assert.NotNull(viewResult);

            _marketingLibrary.Received().AddVoucher(voucher);
            _transactionLibrary.Received().ExecuteBasketPipeline();

        }
    }
}
