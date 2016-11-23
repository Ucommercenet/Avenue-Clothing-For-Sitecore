define('jsPaymentPicker', ['jquery', 'jsConfig'], function ($, config) {
    'use strict';

    // declared with `var`, must be "private"
    var classSelector = ".js-payment-picker";

    /** START OF PUBLIC API **/

    var jsPaymentPicker = {};

    jsPaymentPicker.init = function () {
        config.$rootSelector.find(classSelector)
				.on("change", (function () {
				    config.$triggerEventSelector.trigger("payment-method-changed", {
				        paymentMethodId: $(this).val()
				    });
				}));
    };

    //Make a global event init complete, when all components have been loaded
    jsPaymentPicker.initCompleted = function () {
        var value = config.$rootSelector.find(classSelector + ":checked").val();

        config.$triggerEventSelector.trigger("payment-method-changed", {
            paymentMethodId: value
        });
    };

    /** END OF PUBLIC API **/

    return jsPaymentPicker;
});