﻿define('jsFacets', ['jquery', 'jsConfig'], function ($, config) {
    'use strict';

    // declared with `var`, must be "private"
    var classSelector = ".js-facets";

    function createQueryString() {
        var queryStrings = {};
        var baseUrl = window.location.href.split('?')[0] + '?';
        var allChecked = $(classSelector + ':checked');
        allChecked.each(function () {
            var key = $(this).attr('data-facets-name');
            if (queryStrings[key] == null) {
                queryStrings[key] = $(this).attr('data-facets-value').toString() + '|';
            } else {
                queryStrings[key] += $(this).attr('data-facets-value').toString() + '|';
            }
        });

        for (var propertyName in queryStrings) {
            baseUrl += propertyName + '=' + queryStrings[propertyName] + '&';
        }
        var newUrl = baseUrl.substring(0, baseUrl.length - 1);

        window.location.href = newUrl;
    }

    function ensureCheckboxesAreChecked() {
        var result = {}, queryString = location.search.slice(1),
            re = /([^&=]+)=([^&]*)/g, m;

        while (m = re.exec(queryString)) {
            result[decodeURIComponent(m[1])] = decodeURIComponent(m[2]);
        }

        var params = result;

        for (var propertyName in params) {
            var value = params[propertyName].split('|');

            for (var i = 0; i < value.length - 1; i++) {
                var filter = '.js-facets[data-facets-name="' + propertyName + '"][data-facets-value="' + value[i] + '"]';
                var checkbox = $(filter);
                if (checkbox != null) {
                    checkbox.attr("checked", true);
                }
            }
        }
    }

    /** START OF PUBLIC API **/

    var jsFacets = {};

    jsFacets.init = function () {
        ensureCheckboxesAreChecked();
        config.$rootSelector.find(classSelector).click(createQueryString);
    };

    /** END OF PUBLIC API **/

    return jsFacets;
});