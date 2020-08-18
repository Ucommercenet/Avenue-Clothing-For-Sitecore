using System;
using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Transaction.ViewModels;
using Ucommerce.Api;

namespace AvenueClothing.Project.Transaction.Controllers
{
	public class VoucherController : BaseController
	{
		private readonly IMarketingLibrary _marketingLibrary;
		private readonly ITransactionLibrary _transactionLibrary;

		public VoucherController(ITransactionLibrary transactionLibrary, IMarketingLibrary marketingLibrary)
		{
		    _transactionLibrary = transactionLibrary;
		    _marketingLibrary = marketingLibrary;
		}

		public ActionResult Rendering()
		{
			var voucherViewModel = new VoucherViewModel()
			{
				VoucherUrl = Url.Action("AddVoucher"),
				InputClassSelector = "js-voucher-input-" + Guid.NewGuid(),
				ButtonClassSelector = "js-voucher-button-" + Guid.NewGuid()
			};

			return View(voucherViewModel);
		}

		[HttpPost]
		public ActionResult AddVoucher(string voucher)
		{
            bool success = _marketingLibrary.AddVoucher(voucher);
			_transactionLibrary.ExecuteBasketPipeline();

			return Json(new { voucher, success });
		}
	}
}