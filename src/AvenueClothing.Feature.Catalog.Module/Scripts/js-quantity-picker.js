var QuantityPicker = (function () {

    // declared with `var`, must be "private"
    var classSelector = ".js-quantity-picker";

    var publicScope = {
        init: function (rootSelector) {
            $(rootSelector).find(classSelector)
                .changed(function () {
                    var $picker = $(this);
                    var productSku = $picker.data("product-sku");

                    $(document).trigger("quantity-changed", { productSku: productSku });
                });
        }
    };

    return publicScope;

})();