using System.Web.Mvc;
using AvenueClothing.Feature.Transaction.Controllers;
using AvenueClothing.Feature.Transaction.ViewModels;
using NSubstitute;
using UCommerce.Marketing;
using UCommerce.Transactions;
using Xunit;

namespace AvenueClothing.Feature.Tests
{
    public class VoucherControllerTests
    {
        private readonly MarketingLibraryInternal _marketingLibraryInternal;
        private readonly TransactionLibraryInternal _transactionLibraryInternal;
        private readonly VoucherController _controller;

        public VoucherControllerTests()
        {
            _transactionLibraryInternal = Substitute.For<TransactionLibraryInternal>(null, null, null, null, null, null,
                 null, null, null, null, null);
            _marketingLibraryInternal = Substitute.For<MarketingLibraryInternal>(null, null, null);
      
            _controller = new VoucherController(_transactionLibraryInternal, _marketingLibraryInternal);

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

            _marketingLibraryInternal.Received().AddVoucher(voucher);
            _transactionLibraryInternal.Received().ExecuteBasketPipeline();

        }
    }
}
