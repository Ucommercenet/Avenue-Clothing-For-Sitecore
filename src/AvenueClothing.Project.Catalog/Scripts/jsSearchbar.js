define('jsSearchbar', ['jquery', 'jsConfig'], function ($, config) {
    'use strict';

    /** Behaviour on Tablet/Mobile screen**/
    var classSelectorMobile = ".search-on-small-screen.hide-on-large-screen";
    var searchForm = ".search-form";
    var navbrand = ".navbar-brand";
    var clicked = false;


    var jsSearchbar = {};

    jsSearchbar.init = function () {
        $(classSelectorMobile).click(function () {
            if (!clicked) {
                $(navbrand).innerHTML = "";
                $(searchForm).css("display", "inline-block");
                clicked = true;
            } else {
                $(searchForm).css("display", "none");
                clicked = false;
            }
        });
    };
    return jsSearchbar;
});