define('jsPriceCalculation', ['jquery', 'jsConfig'], function ($, config) {
    'use strict';

    // declared with `var`, must be "private"
    var classSelector = ".js-price-calculation";


    var productVariantChanged = function (event, data) {

        var $priceCalc = $(classSelector);

        var productSku = $priceCalc.data("product-sku");
        var categoryGuid = $priceCalc.data("category-guid");
        var catalogGuid = $priceCalc.data("catalog-guid");
        var calculateVariantPriceUrl = $priceCalc.data("variant-price-calculation-url");

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

    var requestPrice = function (classSelector, productVariantChanged, productSku, catalogGuid, categoryGuid, calculatePriceUrl, productId) {
        config.$rootSelector.find(classSelector)
            .on("product-variant-changed", productVariantChanged)
            .ready(function () {

                $.ajax({
                    type: "POST",
                    url: calculatePriceUrl,
                    data: {
                        ProductSku: productSku,
                        CatalogId: catalogGuid,
                        CategoryId: categoryGuid
                    },
                    dataType: "json",
                    success: function (data, element) {
                        var yourPrice = data.YourPrice;
                        var tax = data.Tax;
                        var discount = data.Discount;

                        var spinnerSelector = '.spinner.' + productId;
                        $(spinnerSelector).css("display", "none");

                        var priceSelector = '.item-price.' + productId;
                        $(priceSelector).text(yourPrice);

                        var productSelector = '.js-price-calculation.' + productId;
                        var taxSelector = $(productSelector).find('.tax');
                        // $('.tax').text('Incl. ' + tax);
                        taxSelector.text('Incl. ' + tax);

                        if (discount && discount > 0) {
                            $(priceSelector).css("text-decoration", "line-through");
                            var discountSelector = '.discount.' + productId;
                            $(discountSelector).text(discount);
                        }
                    }
                });
            });
    }

    /** START OF PUBLIC API **/

    var jsPriceCalculation = {};
    jsPriceCalculation.init = function () {
        var $priceCalc = $(classSelector);
        for (var i = 0; i < $priceCalc.length; i++) {

            var productSku = $priceCalc[i].dataset.productSku;
            var categoryGuid = $priceCalc[i].dataset.categoryGuid;
            var catalogGuid = $priceCalc[i].dataset.catalogGuid;
            var calculatePriceUrl = $priceCalc[i].dataset.priceCalculationUrl;
            var productId = $priceCalc[i].dataset.productId;

            requestPrice('.' + $priceCalc[i].className.replace(" ", "."), productVariantChanged, productSku, catalogGuid, categoryGuid, calculatePriceUrl, productId);
        }
    };

    /** END OF PUBLIC API **/

    return jsPriceCalculation;
});