var QuantityPicker = (function () {

    // declared with `var`, must be "private"
    var classSelector = ".js-quantity-picker";

    var publicScope = {
        init: function ($rootSelector, $triggerEventSelector) {
            $rootSelector.find(classSelector)
                .change(function () {
                    var $picker = $(this);
                    var productSku = $picker.data("product-sku");
                    var productQuantity = $picker.val();

                    $triggerEventSelector.trigger("product-quantity-changed", { productSku: productSku, productQuantity: productQuantity });
                });
        }
    };

    return publicScope;

})();