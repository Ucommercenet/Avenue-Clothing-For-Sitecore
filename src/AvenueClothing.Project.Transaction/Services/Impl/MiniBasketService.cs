using System.Linq;
using AvenueClothing.Project.Transaction.ViewModels;
using Ucommerce;
using Ucommerce.Api;

namespace AvenueClothing.Project.Transaction.Services.Impl
{
	public class MiniBasketService : IMiniBasketService
	{
		private readonly ITransactionLibrary _transactionLibrary;

		public MiniBasketService(ITransactionLibrary transactionLibrary)
		{
			_transactionLibrary = transactionLibrary;
		}

		public MiniBasketRefreshViewModel Refresh()
		{
			var viewModel = new MiniBasketRefreshViewModel
			{
				IsEmpty = true
			};

			if (!_transactionLibrary.HasBasket())
			{
				return viewModel;
			}

			var purchaseOrder = _transactionLibrary.GetBasket();

			var quantity = purchaseOrder.OrderLines.Sum(x => x.Quantity);

			var total = purchaseOrder.OrderTotal.HasValue
				? new Money(purchaseOrder.OrderTotal.Value, purchaseOrder.BillingCurrency.ISOCode)
				: new Money(0, purchaseOrder.BillingCurrency.ISOCode);

			viewModel.NumberOfItems = quantity.ToString();
			viewModel.IsEmpty = quantity == 0;
			viewModel.Total = total.ToString();

			return viewModel;
		}
	}
}