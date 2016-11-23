define('jsShippingPicker', ['jquery', 'jsConfig'], function ($, config) {
    'use strict';

    // declared with `var`, must be "private"
    var classSelector = ".js-mini-basket";

    /** START OF PUBLIC API **/

    var jsShippingPicker = {};

    jsShippingPicker.init = function () {
        config.$rootSelector.find(classSelector)
				.on("change", (function () {
				    config.$triggerEventSelector.trigger("shipping-method-changed", {
				        shippingMethodId: $(this).val()
				    });
				}));
    };

    //Make a global event init complete, when all components have been loaded
    jsShippingPicker.initCompleted = function () {
        var value = config.$rootSelector.find(classSelector + ":checked").val();

        config.$triggerEventSelector.trigger("shipping-method-changed", {
            shippingMethodId: value
        });
    };

    /** END OF PUBLIC API **/

    return jsShippingPicker;
});