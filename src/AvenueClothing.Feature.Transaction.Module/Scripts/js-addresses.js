var Addresses = (function () {
	var classSelector = ".js-addresses";


	var toggleShippingAddress = function () {
		var shippingAddressElement = $(classSelector + ".Shipping");
		if (shippingAddressElement == null) return;

		if (shippingAddressElement.is(":visible")) {
			shippingAddressElement.hide();
		} else {
			shippingAddressElement.show();
		}
	};

	if ($(classSelector)) {
		var billingAddressContentElement = $(classSelector + ".Billing" + " .well");
		if (billingAddressContentElement == null) return;

		var checkbox = document.createElement('input');
		checkbox.type = "checkbox";
		checkbox.addEventListener('click', function () {
			toggleShippingAddress(); return false;
		});

		var label = document.createElement("label");
		label.innerHTML = "Shipping address same as billing";

		billingAddressContentElement[0].appendChild(checkbox);
		billingAddressContentElement[0].appendChild(label);
	}

	
})();

