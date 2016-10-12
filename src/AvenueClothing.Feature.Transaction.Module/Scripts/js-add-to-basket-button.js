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
        var productIsVariant = $button.data("product-is-variant");
        var disableButton = (productIsVariant === 'true' && !productVariantSku) || productQuantity <= 0;
        $button.prop("disabled", disableButton);
    };

    var productVariantChanged = function (event, data) {
        var $button = $(this);

        $button.data("product-variant-sku", data.productVariantSku);

        toogleButton($button);
    };

    var productQuantityChanged = function (event, data) {
        var $button = $(this);

        $button.data("product-quantity", data.productQuantity);

        toogleButton($button);
    };

    var publicScope = {
        init: function (rootSelector) {
            $(rootSelector).find(classSelector)
                .on("product-varaint-changed", productVariantChanged)
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

                        var event = $.Event("basket-changed");
                        $(document).trigger(event);

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