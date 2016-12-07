define('jsVariantPicker', ['jquery', 'jsConfig'], function ($, config) {
    'use strict';
    // declared with `var`, must be "private"
    var classSelector = ".js-variant-picker";

    var getVariantNameValueDictionary = function (currentProductSku) {
        var data = {};

        $(classSelector)
            .each(function () {
                var $picker = $(this);
                var productSku = $picker.data("product-sku");

                if (currentProductSku != productSku) {
                    return;
                }

                var dataVariantName = $picker.data("variant-name");
                var variantValue = $picker.val();

                data[dataVariantName] = variantValue;
            });

        return data;
    };

    var productVariantChanged = function (event, data) {
        var $picker = event.data.$element;

        var productSku = $picker.data("product-sku");
        if (productSku !== data.productSku) {
            return;
        }

        //TODO: disable options that are no longer possible -- server side get possible options
    };

    var jsVariantPicker = {
        init: function () {
            config.$triggerEventSelector.find(classSelector).each(function() {
                config.$triggerEventSelector.on("product-variant-changed", { $element: $(this) }, productVariantChanged);
            });
            config.$rootSelector.find(classSelector)
                .change(function () {
                    var $picker = $(this);
                    var productSku = $picker.data("product-sku");
                    var variantExistsUrl = $picker.data("variant-exists-url");

                    var variantNameValueDictionary = getVariantNameValueDictionary(productSku);

                    $.ajax({
                        type: "POST",
                        url: variantExistsUrl,
                        data:
                        {
                            ProductSku: productSku,
                            VariantNameValueDictionary: variantNameValueDictionary
                        },
                        dataType: "json",
                        success: function (data) {
                            var productVariantSku = data.ProductVariantSku;

                            config.$triggerEventSelector.trigger("product-variant-changed",
                            {
                                productSku: productSku,
                                productVariantSku: productVariantSku
                            });
                        }
                    });
                });
        }
    };

    return jsVariantPicker;

});