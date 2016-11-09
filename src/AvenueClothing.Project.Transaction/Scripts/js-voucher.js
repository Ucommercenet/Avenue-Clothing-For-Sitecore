var Voucher = (function () {

	// declared with `var`, must be "private"
	var classSelector = ".js-voucher";

	//var addVoucher = function ($triggerEventSelector) {
	//	var $button = $(this);
	//	var inputSelector = $button.data("input-class-selector");

	//	var voucherInput = $("#" + inputSelector);
	//}

	var publicScope = {
		init: function ($rootSelector, $triggerEventSelector) {
			var $vouchers = $rootSelector.find(classSelector);

			$vouchers.each(function (index, element) {
				var buttonClassSelector = $(this).data("button-class-selector");

				$(this).find("#" + buttonClassSelector).on("click", function () {
					var $button = $(this);
					var inputSelector = $button.data("input-class-selector");
					var voucher = $("#" + inputSelector).val();
					var addVoucherUrl = $button.data("voucher-url");

					$.ajax({
						type: "POST",
						url: addVoucherUrl,
						data:
						{
							voucher: voucher
						},
						dataType: "json",
						success: function (data) {
							$triggerEventSelector.trigger("basket-changed");
						}
					});
				});
			});
		}
	};

	return publicScope;

})();