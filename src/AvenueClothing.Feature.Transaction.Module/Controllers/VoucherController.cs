using System;
using System.Web.Mvc;
using AvenueClothing.Feature.Transaction.Module.ViewModels;
using AvenueClothing.Foundation.MvcExtensionsModule;
using UCommerce.Api;
using UCommerce.Marketing;
using UCommerce.Transactions;

namespace AvenueClothing.Feature.Transaction.Module.Controllers
{
	public class VoucherController : BaseController
	{
		private readonly MarketingLibraryInternal _marketingLibraryInternal;
		private readonly TransactionLibraryInternal _transactionLibraryInternal;

		public VoucherController(TransactionLibraryInternal transactionLibraryInternal)
		{
			_transactionLibraryInternal = transactionLibraryInternal;
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
			//TODO: user marketing library internal
			MarketingLibrary.AddVoucher(voucher);
			_transactionLibraryInternal.ExecuteBasketPipeline();

			return Json(new { voucher });
		}
	}
}