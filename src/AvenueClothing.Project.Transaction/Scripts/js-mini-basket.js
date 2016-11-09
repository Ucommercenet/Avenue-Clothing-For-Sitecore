var MiniBasket = (function () {

	// declared with `var`, must be "private"
	var classSelector = ".js-mini-basket";

	var basketChanged = function (event, data) {
		var $miniBasket = $(this);

		var emptySelector = $miniBasket.data("mini-basket-empty-selector");
		var notEmptySelector = $miniBasket.data("mini-basket-not-empty-selector");
		var numberOfItemsSelector = $miniBasket.data("mini-basket-number-of-items-selector");
		var totalSelector = $miniBasket.data("mini-basket-total-selector");

		if (data) {
			if (data.IsEmpty) {
				$miniBasket.find(notEmptySelector).hide();
				$miniBasket.find(emptySelector).show();
			} else {
				$miniBasket.find(numberOfItemsSelector).text(data.NumberOfItems);
				$miniBasket.find(totalSelector).text(data.Total);

				$miniBasket.find(notEmptySelector).show();
				$miniBasket.find(emptySelector).hide();
			}
		}
	}
	
	var publicScope = {
		init: function ($rootSelector, $triggerEventSelector) {
			$rootSelector.find(classSelector).on("basket-changed", basketChanged);
		}
	};

	return publicScope;

})();