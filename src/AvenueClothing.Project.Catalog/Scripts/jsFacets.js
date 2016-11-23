define('jsFacets', ['jquery', 'jsConfig', 'urijs'], function ($, config) {
    'use strict';

    // declared with `var`, must be "private"
    //var classSelector = ".js-facets";
    var classSelector = document.querySelectorAll("[data-selector='js-facets']");

    function createQueryString() {
        //TODO: Use urijs to make url
        //https://github.com/medialize/URI.js/
        var queryStrings = {};
        var baseUrl = window.location.href.split('?')[0] + '?';
        var allChecked = $(classSelector + ':checked') ;
        allChecked.each(function () {
            var key = $(this).attr('key');
            if (queryStrings[key] == null) {
                queryStrings[key] = $(this).attr('value').toString() + '|';
            } else {
                queryStrings[key] += $(this).attr('value').toString() + '|';
            }
        });

        for (var propertyName in queryStrings) {
            baseUrl += propertyName + '=' + queryStrings[propertyName] + '&';
        }
        var newUrl = baseUrl.substring(0, baseUrl.length - 1);

        window.location.href = newUrl;
    }

    function ensureCheckboxesAreChecked() {
        //TODO: Decode url with urijs
        //https://github.com/medialize/URI.js/
        var result = {}, queryString = location.search.slice(1),
            re = /([^&=]+)=([^&]*)/g, m;

        while (m = re.exec(queryString)) {
            result[decodeURIComponent(m[1])] = decodeURIComponent(m[2]);
        }

        var params = result;

        for (var propertyName in params) {
            var value = params[propertyName].split('|');

            for (var i = 0; i < value.length - 1; i++) {
                //TODO: Change to use html5 data properties
                var filter = '.filter[key="' + propertyName + '"][value="' + value[i] + '"]';
                var checkbox = $(filter);
                if (checkbox != null) {
                    checkbox.prop('checked', true);
                }
            }
        }
    }

    /** START OF PUBLIC API **/

    var jsFacets = {};

    jsFacets.init = function () {
        ensureCheckboxesAreChecked();
        config.$(classSelector).click(createQueryString);
    };

    /** END OF PUBLIC API **/

    return jsFacets;
});