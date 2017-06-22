define('jsSearchbar', ['jquery'], function ($) {
    'use strict';

    /** Behaviour on Tablet/Mobile screen**/
    var classSelectorMobile = ".search-on-small-screen.hide-on-large-screen";
    var searchForm = "form.search-form";
    var navbrand = ".navbar-brand span";


    var jsSearchbar = {};

    jsSearchbar.init = function () {
        var clicked = false;
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