using System.Web.Mvc;
using Sitecore.Mvc.Controllers;
using UCommerce.Marketing;
using UCommerce.Transactions;

namespace AvenueClothing.Feature.Transaction.Module.Controllers
{
	public class VoucherController : SitecoreController
	{
		private readonly MarketingLibraryInternal _marketingLibraryInternal;
		private readonly TransactionLibraryInternal _transactionLibraryInternal;

		public VoucherController(MarketingLibraryInternal marketingLibraryInternal, TransactionLibraryInternal transactionLibraryInternal)
		{
			_marketingLibraryInternal = marketingLibraryInternal;
			_transactionLibraryInternal = transactionLibraryInternal;
		}

		public ActionResult Rendering()
		{
			return View();
		}

		public ActionResult AddVoucher(string voucher)
		{
			_marketingLibraryInternal.AddVoucher(voucher);
			_transactionLibraryInternal.ExecuteBasketPipeline();

			return Json(new { });

		}
	}
}