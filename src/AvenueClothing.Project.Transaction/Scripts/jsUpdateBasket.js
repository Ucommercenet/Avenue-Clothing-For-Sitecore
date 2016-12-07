define('jsUpdateBasket', ['jquery', 'jsConfig'], function ($, config) {
    'use strict';

    // declared with 'var', must be "private"
    var classSelector = '.js-update-basket';

    /** START OF PUBLIC API **/

    var jsUpdateBasket = {};

    jsUpdateBasket.init = function () {
        config.$rootSelector.find(classSelector).click(function () {
            var $updateBasket = $(this);
            var refreshUrl = $updateBasket.data('refresh-url');
            var orderlines = $('[data-orderline-id]');

            var orderlineArray = [];

            orderlines.each(function (index, element) {
                var orderlineId = element.dataset.orderlineId;
                var orderlineQty = element.value;
                var currentKeyValue = { orderlineId, orderlineQty }

                orderlineArray.push(currentKeyValue);
            });

            $.ajax({
                type: 'POST',
                url: refreshUrl,
                data: {
                    RefreshBasket: orderlineArray
                },
                dataType: 'json',
                success: function (data) {
                    $('[data-orderline]').each(function (index, element) {
                        var orderlineId = element.dataset.orderline;
                    });

                    var orderSubtotal = $updateBasket.data('order-subtotal');
                    $(orderSubtotal).text(data.SubTotal);
                    var taxTotal = $updateBasket.data('order-tax');
                    $(taxTotal).text(data.TaxTotal);
                    var discountTotal = $updateBasket.data('order-discounts');
                    $(discountTotal).text(data.DiscountTotal);
                    var orderTotal = $updateBasket.data('order-total');
                    $(orderTotal).text(data.OrderTotal);
                  
                    config.$triggerEventSelector.trigger("basket-changed", data.MiniBasketRefresh);


                },
                error: function (err) {
                    console.log("Something went wrong...");
                }
            });
        });
    }
    return jsUpdateBasket;
});