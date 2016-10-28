var UpdateBasketButton = (function () {

    var classSelector = ".js-update-basket-button";

    var publicScope = {
        init: function ($rootSelector, $triggerEventSelector) {
            $rootSelector.find(classSelector)
                //.on("product-quantity-changed", quantityChanged)
                .click(function () {
                    var updatedQuantity = (".quantity").value();
                    var orderlineId = (".order-line-id").value();

                    $.ajax({
                        type: "POST",
                        url: updatedQuantityUrl,
                        data:
                        {
                            Quantity: updatedQuantity,
                            OrderLineId: orderlineId
                        },
                        dataType: "json",
                        success: function (data) {
                            $triggerEventSelector.trigger("basket-changed");
                        }
                    });
                });
        }
    };

    return publicScope;

})();