define('jsAddress', ['jquery', 'jsConfig'], function ($, config) {
    'use strict';

    // declared with `var`, must be "private"
    var classSelector = ".js-address";
    var billingClassSelector = ".js-address-billing";
    var shippingClassSelector = ".js-address-shipping";
    var checkboxClassSelector = ".js-address-checkbox";

    var toggleShippingAddress = function () {
        var value = $(this).is(":checked");
        var shippingAddress = $(classSelector).find(shippingClassSelector);

        if (value) {
            shippingAddress.show();
        }
        else {
            shippingAddress.hide();
        }
    };

    /** START OF PUBLIC API **/

    var jsAddress = {};

    jsAddress.init = function () {
        config.$rootSelector.find(checkboxClassSelector).on("change", toggleShippingAddress);
    };

    /** END OF PUBLIC API **/

    return jsAddress;
});