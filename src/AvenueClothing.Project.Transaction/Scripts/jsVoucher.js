define('jsVoucher', ['jquery', 'jsConfig'], function ($, config) {
    'use strict';

    // declared with `var`, must be "private"
    var classSelector = ".js-voucher";

    //var addVoucher = function ($triggerEventSelector) {
    //	var $button = $(this);
    //	var inputSelector = $button.data("input-class-selector");

    //	var voucherInput = $("#" + inputSelector);
    //}

    /** START OF PUBLIC API **/

    var jsVoucher = {};

    jsVoucher.init = function () {
        var $vouchers = config.$rootSelector.find(classSelector);

        $vouchers.each(function (index, element) {
            var buttonClassSelector = $(this).data("button-class-selector");

            $(this).find("#" + buttonClassSelector).on("click", function () {
                var $button = $(this);
                var inputSelector = $button.data("input-class-selector");
                var voucher = $("#" + inputSelector).val();
                var addVoucherUrl = $button.data("voucher-url");

                $.ajax({
                    type: "POST",
                    url: addVoucherUrl,
                    data:
                    {
                        voucher: voucher
                    },
                    dataType: "json",
                    success: function (data) {
                        config.$triggerEventSelector.trigger("basket-changed");
                    }
                });
            });
        });
    };

    /** END OF PUBLIC API **/

    return jsVoucher;
});