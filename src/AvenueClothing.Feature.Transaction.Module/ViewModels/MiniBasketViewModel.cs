using UCommerce;

namespace AvenueClothing.Feature.Transaction.Module.ViewModels
{
	public class MiniBasketViewModel
	{
		public int NumberOfItems { get; set; }
		public Money Total { get; set; }
		public bool IsEmpty { get; set; }
	}
}