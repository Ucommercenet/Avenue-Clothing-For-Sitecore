using UCommerce;

namespace AvenueClothing.Feature.Transaction.Module.ViewModels
{
	public class RefreshMiniBasketViewModel
	{
		public string NumberOfItems { get; set; }
		public string Total { get; set; }
		public bool IsEmpty { get; set; }
	}
}