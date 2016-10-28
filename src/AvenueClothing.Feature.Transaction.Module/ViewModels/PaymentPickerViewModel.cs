using System.Collections.Generic;
using System.Web.Mvc;

namespace AvenueClothing.Feature.Transaction.Module.ViewModels
{
	public class PaymentPickerViewModel
	{
		public IList<SelectListItem> AvailablePaymentMethods { get; set; }

		public int SelectedPaymentMethodId { get; set; }

		public string ShippingCountry { get; set; }
	}
}