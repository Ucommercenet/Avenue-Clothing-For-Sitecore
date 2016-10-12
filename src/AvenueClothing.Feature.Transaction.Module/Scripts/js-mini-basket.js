var MiniBasket = (function () {

	// declared with `var`, must be "private"
	var privateMethod = function () { };

	var publicScope = {
		init: function (rootSelector) {
			//$(rootSelector).find(".js-mini-basket")
		},
		basketChanged: function (rootSelector) {
			alert('TODO: Update mini basket');

			$(rootSelector).find(".js-mini-basket")
			   .each(function () {
			   	var $miniBasket = $(this);
			   	var validateProductExistsUrl = $(this).data("validate-product-exists-url");
			   	$.ajax({
			   		type: "GET",
			   		url: validateProductExistsUrl,
			   		data:
					{
						ProductSku: product.productSku,
						VariantSku: product.variantSku
					},
			   		dataType: "json"
			   	}
				.done(function (data) {
					if (data.IsEmpty) {
						$miniBasket.find("js-mini-basket-not-empty").addClass("hidden-*");
						$miniBasket.find("js-mini-basket-empty").removeClass("hidden-*");
					} else {
						$miniBasket.find(".js-mini-basket-number-of-items").text(data.NumberOfItems);
						$miniBasket.find(".js-mini-basket-total").text(data.Total);

						$miniBasket.find("js-mini-basket-not-empty").removeClass("hidden-*");
						$miniBasket.find("js-mini-basket-empty").addClass("hidden-*");
					}
				})
				.fail(function () {
					alert("Whoops...");
				})
				.always(function () {
					//No-op
				}));
			   });

			


		}
	};

	return publicScope;

})();