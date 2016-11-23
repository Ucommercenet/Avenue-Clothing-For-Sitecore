define('jsReviewForm', ['jquery', 'jsConfig'], function ($, config) {
    'use strict';

    // declared with `var`, must be "private"
    var classSelector = ".js-review-form";

    function wireupRatings(radios) {
        $('#review-form').addClass("display-none");
        $('label', radios).each(function () {
            var t = $(this);
            t.addClass('display-inline');
            t.addClass('off');
            $('input:radio', t).addClass("display-none");
            setStarHoverOutState($('i', t));
            t.hover(function () {
                var parent = $(this);
                var labels = parent.prevAll('label');
                setStarHoverState($('i', labels));
                setStarHoverState($('i', parent));
            }, function () {
                var parent = $(this);
                var labels = parent.prevAll('label');
                if (!parent.hasClass('selected')) {
                    setStarHoverOutState($('i', labels));
                    setStarHoverOutState($('i', parent));
                }
            });
            t.click(function () {
                var parent = $(this);
                parent.addClass('selected');
                $('#review-form').slideDown();
            });
        });
    };

    function setStarHoverState(label) {
        label.addClass('fa-star').removeClass('fa-star-o');
    }
    function setStarHoverOutState(label) {
        label.addClass('fa-star-o').removeClass('fa-star');
    }

    /** START OF PUBLIC API **/

    var jsReviewForm = {};

    jsReviewForm.init = function () {
        wireupRatings(config.$rootSelector.find(classSelector));
    };

    /** END OF PUBLIC API **/

    return jsReviewForm;
});