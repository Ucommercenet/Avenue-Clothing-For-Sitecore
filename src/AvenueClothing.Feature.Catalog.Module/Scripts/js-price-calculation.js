var PriceCalculation = (function () {
 
    var classSelector = ".js-price-calculation";

    var publicScrope = {
        init: function($rootSelector, $triggerEventSelector) {
            $rootSelector.find(classSelector).ready(function() {
                //var $priceCalc = $(".js-price-calculation");
                var $priceCalc = $(classSelector);

                var productSKU = $priceCalc.data("product-sku");
                var categoryGuid = $priceCalc.data("category-guid");
                var catalogGuid = $priceCalc.data("catalog-guid");
                var calculatePriceUrl = $priceCalc.data("price-calculation-url");

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
            });
        }
    }
    return publicScrope;
})();