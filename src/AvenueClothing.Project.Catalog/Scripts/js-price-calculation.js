var PriceCalculation = (function () {
 
    var classSelector = ".js-price-calculation";

    var $priceCalc = $(classSelector);

    var productSKU = $priceCalc.data("product-sku");
    var categoryGuid = $priceCalc.data("category-guid");
    var catalogGuid = $priceCalc.data("catalog-guid");
    var calculatePriceUrl = $priceCalc.data("price-calculation-url");
    var calculateVariantPriceUrl = $priceCalc.data("variant-price-calculation-url");

    var productVariantChanged = function(event, data) {
        var $priceCalc = $(classSelector);

        $.ajax({
            type: "POST",
            url: calculateVariantPriceUrl,
            data: {
                ProductSku: productSKU,
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

    var publicScrope = {
        init: function($rootSelector, $triggerEventSelector) {
            $rootSelector.find(classSelector)
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
        }
    }
    return publicScrope;
})();