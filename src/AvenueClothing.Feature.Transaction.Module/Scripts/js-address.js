var Addresses = (function () {
	var classSelector = ".js-address";
	var billingClassSelector = ".js-address-billing";
	var shippingClassSelector = ".js-address-shipping";
	var checkboxClassSelector = ".js-address-checkbox";


	var toggleShippingAddress = function () {
		var value = $(this).is(":checked");
		var shippingAddress = $(classSelector).find(shippingClassSelector);

		if (value) {
			shippingAddress.show();
		}
		else {
			shippingAddress.hide();
		}
	};

	var publicScope = {
		init: function ($rootSelector, $triggerEventSelector) {
			$rootSelector.find(checkboxClassSelector).on("change", toggleShippingAddress);
		}
	};

	return publicScope;
})();

