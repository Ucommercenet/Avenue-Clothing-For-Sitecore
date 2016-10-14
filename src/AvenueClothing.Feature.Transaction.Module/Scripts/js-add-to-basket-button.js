var AddToBasketButton = (function () {

    // declared with `var`, must be "private"
    var classSelector = ".js-add-to-basket-button";
    var confirmationMessageTimer;

    var showConfirmationMessage = function ($button) {

        var confirmationMessageId = $button.data("confirmation-message-id");
        var confirmationMessageTimeoutInMillisecs = $button.data("confirmation-message-timeout-in-millisecs");

        var $message = $("#" + confirmationMessageId);

        $message.slideDown();

        clearTimeout(confirmationMessageTimer);

        confirmationMessageTimer = setTimeout(function () {
            $message.slideUp();
        }, confirmationMessageTimeoutInMillisecs);
    };

    var toogleButton = function ($button) {
        var productVariantSku = $button.data("product-variant-sku");
        var productQuantity = $button.data("product-quantity");
        var isProductFamily = $button.data("is-product-family");
        var disableButton = (isProductFamily === 'True' && !productVariantSku) || productQuantity <= 0;
        $button.prop("disabled", disableButton);
    };

    var productVariantChanged = function (event, data) {
        var $button = $(this);

        var productSku = $button.data("product-sku");
        if (productSku !== data.productSku) {
            return;
        }

        $button.data("product-variant-sku", data.productVariantSku);

        toogleButton($button);
    };

    var productQuantityChanged = function (event, data) {
        var $button = $(this);

        var productSku = $button.data("product-sku");
        if (productSku !== data.productSku) {
            return;
        }

        $button.data("product-quantity", data.productQuantity);

        toogleButton($button);
    };

    var publicScope = {
        init: function ($rootSelector, $triggerEventSelector) {
            $rootSelector.find(classSelector)
                .on("product-variant-changed", productVariantChanged)
                .on("product-quantity-changed", productQuantityChanged)
                .click(function () {
                    var $button = $(this);

                    var productSku = $button.data("product-sku");
                    var addToBasketUrl = $button.data("add-to-basket-url");
                    var productVariantSku = $button.data("product-variant-sku");
                    var productQuantity = $button.data("product-quantity");
                
                    $.ajax({
                        type: "POST",
                        url: addToBasketUrl,
                        data:
                        {
                            Quantity: productQuantity,
                            ProductSku: productSku,
                            VariantSku: productVariantSku
                        },
                        dataType: "json"
                    }
                    .done(function () {

                        $triggerEventSelector.trigger("basket-changed");

                        showConfirmationMessage($button);
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