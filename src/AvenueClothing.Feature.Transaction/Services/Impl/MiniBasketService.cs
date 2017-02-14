using System.Linq;
using AvenueClothing.Feature.Transaction.ViewModels;
using UCommerce;
using UCommerce.Transactions;

namespace AvenueClothing.Feature.Transaction.Services.Impl
{
	public class MiniBasketService : IMiniBasketService
	{
		private readonly TransactionLibraryInternal _transactionLibraryInternal;

		public MiniBasketService(TransactionLibraryInternal transactionLibraryInternal)
		{
			_transactionLibraryInternal = transactionLibraryInternal;
		}

		public MiniBasketRefreshViewModel Refresh()
		{
			var viewModel = new MiniBasketRefreshViewModel
			{
				IsEmpty = true
			};

			if (!_transactionLibraryInternal.HasBasket())
			{
				return viewModel;
			}

			var purchaseOrder = _transactionLibraryInternal.GetBasket(false).PurchaseOrder;

			var quantity = purchaseOrder.OrderLines.Sum(x => x.Quantity);

			var total = purchaseOrder.OrderTotal.HasValue
				? new Money(purchaseOrder.OrderTotal.Value, purchaseOrder.BillingCurrency)
				: new Money(0, purchaseOrder.BillingCurrency);

			viewModel.NumberOfItems = quantity.ToString();
			viewModel.IsEmpty = quantity == 0;
			viewModel.Total = total.ToString();

			return viewModel;
		}
	}
}