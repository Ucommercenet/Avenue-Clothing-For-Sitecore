using System.Collections.Generic;
using System.Web.Mvc;

namespace AvenueClothing.Feature.Transaction.Module.ViewModels
{
	public class AddressSaveViewModel
	{
		public AddressSaveViewModel() 
        {
			ShippingAddress = new AddressViewModel();
			BillingAddress = new AddressViewModel();
            IsShippingAddressDifferent = false;
        }
		public AddressViewModel ShippingAddress { get; set; }

		public AddressViewModel BillingAddress { get; set; }

        public bool IsShippingAddressDifferent { get; set; }
	}
}