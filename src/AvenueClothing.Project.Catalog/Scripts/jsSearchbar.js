define('jsSearchbar', ['jquery', 'jsConfig'], function ($, config) {
    'use strict';

    /** Behaviour on Tablet/Mobile screen**/
    var classSelectorMobile = ".search-on-small-screen.hide-on-large-screen";
    var searchForm = ".search-form";
    var navbrand = ".navbar-brand span";
    var clicked = false;


    var jsSearchbar = {};

    jsSearchbar.init = function () {
        $(classSelectorMobile).click(function () {
            if (!clicked) {
                $(navbrand).removeClass("display-inline");

                $(navbrand).addClass("display-none");
                $(searchForm).addClass("display-inline");
                clicked = true;
            } else {
                $(searchForm).removeClass("display-inline");
                $(navbrand).addClass("display-inline");
                $(searchForm).addClass("display-none");
                clicked = false;
            }
        });
    };
    return jsSearchbar;
});