define('jsUpdateBasket', ['jquery', 'jsConfig'], function ($, config) {
    'use strict';

    // declared with 'var', must be "private"
    var classSelector = '.js-update-basket';

    /** START OF PUBLIC API **/

    var jsUpdateBasket = {};

    jsUpdateBasket.init = function () {
        config.$rootSelector.find(classSelector).click(function () {
            var $picker = $(this);
            var refreshUrl = $picker.data('refresh-url');
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

                    $('[data-order-subtotal]').text(data.SubTotal);
                    $('[data-order-tax]').text(data.TaxTotal);
                    $('[data-order-discounts]').text(data.DiscountTotal);
                    $('[data-order-total]').text(data.OrderTotal);
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