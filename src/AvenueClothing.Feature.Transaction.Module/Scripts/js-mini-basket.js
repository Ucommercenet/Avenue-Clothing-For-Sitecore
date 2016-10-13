var MiniBasket = (function () {

	// declared with `var`, must be "private"
	var classSelector = ".js-mini-basket";

	var basketChanged = function (event, data) {
		var $miniBasket = $(this);

		var emptySelector = $miniBasket.data("mini-basket-empty-selector");
		var notEmptySelector = $miniBasket.data("basket-not-empty-selector");
		var numberOfItemsSelector = $miniBasket.data("mini-basket-number-of-items-selector");
		var totalSelector = $miniBasket.data("mini-basket-total-selector");
        var refreshUrl = $(this).data("refresh-url");

        $.ajax({
			type: "GET",
			url: refreshUrl,
			dataType: "json"
		}
			.done(function (data) {
				if (data.IsEmpty) {
					$miniBasket.find(notEmptySelector).hide();
					$miniBasket.find(emptySelector).show();
				} else {
					$miniBasket.find(numberOfItemsSelector).text(data.NumberOfItems);
					$miniBasket.find(totalSelector).text(data.Total);

					$miniBasket.find(notEmptySelector).show();
					$miniBasket.find(emptySelector).hide();
				}
			})
			.fail(function () {
				alert("Whoops...");
			})
			.always(function () {
				//No-op
			}));
	}


	var publicScope = {
		init: function (rootSelector) {
		    $(rootSelector).find(classSelector).on("basket-changed", basketChanged);
		}
	};

	return publicScope;

})();