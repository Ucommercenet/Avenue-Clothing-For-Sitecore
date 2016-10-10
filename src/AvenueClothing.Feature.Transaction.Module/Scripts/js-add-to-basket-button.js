var AddToBasketButton = (function () {

    // declared with `var`, must be "private"
    var privateMethod = function () { };

    var publicScope = {
        init: function (rootSelector) {
            $(rootSelector).find(".js-add-to-basket-button").click(function () {
                var quantity = $(this).data("quantity");
                var productSku = $(this).data("product-sku");
                var variantSku = $(this).data("variant-sku");
                var addToBasketUrl = $(this).data("add-to-basket-url");

                $.ajax({
                    type: "POST",
                    url: addToBasketUrl,
                    data:
                    {
                        Quantity: quantity,
                        ProductSku: productSku,
                        VariantSku: variantSku
                    },
                    dataType: "json"
                }
                .done(function () {
                    MiniBasket.basketChanged();
                    alert('TODO: Success confirmation');
                })
                .fail(function () {
                    alert("Whoops...");
                })
                .always(function () {
                    //No-op
                }));
            });
        },
        selectProduct: function (rootSelector, product) {
            $(rootSelector).find(".js-add-to-basket-button")
                .data("quantity", product.quantity)
                .data("product-sku", product.productSku)
                .data("variant-sku", product.variantSku)
                .prop("disabled", true)
                .each(function () {
                    var $button = $(this);
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
                    .done(function () {
                        $button.prop("disabled", false);
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