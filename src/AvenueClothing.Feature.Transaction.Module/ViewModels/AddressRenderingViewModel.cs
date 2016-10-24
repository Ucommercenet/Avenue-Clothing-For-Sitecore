using System.Collections.Generic;
using System.Web.Mvc;

namespace AvenueClothing.Feature.Transaction.Module.ViewModels
{
	public class AddressRenderingViewModel
	{
		public AddressRenderingViewModel() 
        {
			ShippingAddress = new AddressViewModel();
			BillingAddress = new AddressViewModel();
			AvailableCountries = new List<SelectListItem>();
            IsShippingAddressDifferent = false;
        }
		public AddressViewModel ShippingAddress { get; set; }

		public AddressViewModel BillingAddress { get; set; }

        public bool IsShippingAddressDifferent { get; set; }

		public IList<SelectListItem> AvailableCountries { get; set; }
	}
}