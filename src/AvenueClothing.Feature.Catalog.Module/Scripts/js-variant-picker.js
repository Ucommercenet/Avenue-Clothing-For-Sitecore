var VariantPicker = (function () {

    // declared with `var`, must be "private"

    var publicScope = {
        init: function (rootSelector) {
            $(rootSelector).find(".js-variant-picker").change(function () {
                
                var variantPickerName = $(this).data("data-variant-name");
                var variantPickerProductSku = $(this).data("data-product-sku");

                //TODO: Url for check valid choices and get variant SKU

                $.ajax({
                    type: "GET",
                    url: addToBasketUrl,
                    data:
                    {
                        Quantity: quantity,
                        ProductSku: productSku,
                        VariantSku: variantSku
                    },
                    dataType: "json"
                }
                .done(function (data) {
                    
                    //TODO: data-product-sku skal sættes på add button fra start!

                    if (data.variantSku) {
                        AddToBasketButton.selectProduct($(document));
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