define('jsPriceCalculation', ['jquery', 'jsConfig'], function ($, config) {
    'use strict';

    // declared with `var`, must be "private"
    var classSelector = ".js-price-calculation";

    var $priceCalc = $(classSelector);

    var productSku = $priceCalc.data("product-sku");
    var categoryGuid = $priceCalc.data("category-guid");
    var catalogGuid = $priceCalc.data("catalog-guid");
    var calculatePriceUrl = $priceCalc.data("price-calculation-url");
    var calculateVariantPriceUrl = $priceCalc.data("variant-price-calculation-url");

    var productVariantChanged = function (event, data) {

        $.ajax({
            type: "POST",
            url: calculateVariantPriceUrl,
            data: {
                ProductSku: productSku,
                CatalogId: catalogGuid,
                CategoryId: categoryGuid,
                ProductVariantSku: data.productVariantSku
            },
            dataType: "json",
            success: function (data) {
                var yourPrice = data.YourPrice;
                var yourTax = data.Tax;
                $('.item-price').text(yourPrice);
                $('.tax').text('Incl. ' + yourTax);
            }
        });
    }

    /** START OF PUBLIC API **/

    var jsPriceCalculation = {};

    jsPriceCalculation.init = function () {
        config.$rootSelector.find(classSelector)
                .ready(function () {

                    $.ajax({
                        type: "POST",
                        url: calculatePriceUrl,
                        data: {
                            ProductSku: productSKU,
                            CatalogId: catalogGuid,
                            CategoryId: categoryGuid,
                        },
                        dataType: "json",
                        success: function (data) {
                            var yourPrice = data.YourPrice;
                            var tax = data.Tax;

                            $('.item-price').text(yourPrice);
                            $('.tax').text('Incl. ' + tax);
                        }
                    });
                }).on("product-variant-changed", productVariantChanged);
    };

    /** END OF PUBLIC API **/

    return jsPriceCalculation;
});