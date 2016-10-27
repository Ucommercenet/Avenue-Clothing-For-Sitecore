var ShippingPicker = (function () {

	// declared with `var`, must be "private"
	var classSelector = ".js-shipping-picker";

	var publicScope = {
		init: function ($rootSelector, $triggerEventSelector) {
			$rootSelector.find(classSelector)
				.on("change", (function () {
					$triggerEventSelector.trigger("shipping-method-changed", {
						shippingMethodId: $(this).val()
					});
				}));
		},

		initCompleted: function ($rootSelector, $triggerEventSelector) {
			var value = $rootSelector.find(classSelector + ":checked").val();

			$triggerEventSelector.trigger("shipping-method-changed", {
				shippingMethodId: value
			});
		}
	};

	return publicScope;

})();