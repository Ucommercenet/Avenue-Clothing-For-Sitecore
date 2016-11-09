var PaymentPicker = (function () {

	// declared with `var`, must be "private"
	var classSelector = ".js-payment-picker";

	var publicScope = {
		init: function ($rootSelector, $triggerEventSelector) {
			$rootSelector.find(classSelector)
				.on("change", (function () {
					$triggerEventSelector.trigger("payment-method-changed", {
						paymentMethodId: $(this).val()
					});
				}));
		},

		initCompleted: function ($rootSelector, $triggerEventSelector) {
			var value = $rootSelector.find(classSelector + ":checked").val();

			$triggerEventSelector.trigger("payment-method-changed", {
				paymentMethodId: value
			});
		}
	};

	return publicScope;

})();