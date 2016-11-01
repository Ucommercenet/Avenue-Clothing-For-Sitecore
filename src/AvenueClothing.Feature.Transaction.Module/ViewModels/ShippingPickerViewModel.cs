using System.Collections.Generic;
using System.Web.Mvc;

namespace AvenueClothing.Feature.Transaction.Module.ViewModels
{
	public class ShippingPickerViewModel
	{
		public ShippingPickerViewModel()
		{
			AvailableShippingMethods = new List<SelectListItem>();
		}

		public IList<SelectListItem> AvailableShippingMethods { get; set; }

		public int SelectedShippingMethodId { get; set; }

		public string ShippingCountry { get; set; }
	}
}