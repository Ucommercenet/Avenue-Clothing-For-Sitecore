define('jsReviewForm', ['jquery', 'jsConfig', 'jquery.validate'], function ($, config) {
    'use strict';

    // declared with `var`, must be "private"
    var classSelector = ".js-review-form";
    (function ($) {
        $.fn.isAfter = function (sel) {
            return this.prevAll().filter(sel).length !== 0;
        };

        $.fn.isBefore = function (sel) {
            return this.nextAll().filter(sel).length !== 0;
        };
    })(jQuery);

    var selected;
    function wireupRatings(radios) {
        $('#review-form').addClass("display-none");
        $('label', radios).each(function () {
            var t = $(this);
            t.addClass('display-inline');
            t.addClass('off');
            $('input:radio', t).addClass("display-none");
            setStarHoverOutState($('i', t));

            t.hover(function () {
                if (!t.isBefore(selected)) {
                    var parent = $(this);
                    var labels = parent.prevAll('label');
                    setStarHoverState($('i', labels));
                    setStarHoverState($('i', parent));
                }
            }, function () {
                if (!t.isBefore(selected)) {
                    var parent = $(this);
                    var labels = parent.prevAll('label');
                    if (!parent.hasClass('selected')) {
                        setStarHoverOutState($('i', labels));
                        setStarHoverOutState($('i', parent));
                    }
                }
                $('label', radios).each(function () {
                    var t = $(this);
                    if (t.hasClass('selected')) {
                        setStarHoverState($('i', t.prevAll('label')));
                        setStarHoverState($('i', t));
                        setStarHoverOutState($('i', t.nextAll('label')));
                    }
                });
            });
            t.click(function () {
                var parent = $(this);
                $('label', radios).each(function () {
                    var t = $(this);
                    setStarHoverOutState($('i', t));
                    t.removeClass('selected');
                    if (t.hasClass('selected')) {
                        setStarHoverState($('i', t));
                    }
                });
                parent.addClass('selected');
                selected = parent;
                $('#review-form').slideDown();
                $('label', radios).each(function () {
                    var t = $(this);
                    if (t.hasClass('selected')) {
                        setStarHoverState($('i', t.prevAll('label')));
                        setStarHoverState($('i', t));
                        setStarHoverOutState($('i', t.nextAll('label')));
                    }
                });

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
    var submitReviewUrl = $('[data-submit-url]').data('submit-url');
    var $reviewForm = $(classSelector);

    jsReviewForm.init = function () {
        wireupRatings(config.$rootSelector.find(classSelector));


        var submit = $reviewForm.data('submit-button');

        $(submit).off().click(function (e) {
            e.preventDefault();
            var serializedFormData = $reviewForm.serializeArray();
            var values = {};

            $reviewForm.validate({
                errorElement: "label",
                errorClass: "error-custom",
                highlight: function (tag) {
                    $(tag).addClass('error-input');
                    return false;
                },
                success: function (tag) {
                    $(tag).closest('input').removeClass('error-custom');
                }
            });
            if ($reviewForm.valid()) {
                $.each(serializedFormData, function (i, field) {
                    values[field.name] = field.value;
                });
                $.ajax({
                    type: 'POST',
                    url: submitReviewUrl,
                    data: {
                        Name: values['Name'],
                        Email: values['Email'],
                        CategoryGuid: values['CategoryGuid'],
                        Comments: values['Comments'],
                        ProductGuid: values['ProductGuid'],
                        Rating: parseInt(values['Rating']),
                        Title: values['Title']
                    },
                    success: function (data) {
                        config.$triggerEventSelector.trigger("review-added", data);

                    }
                });
                $reviewForm[0].reset();
            };
        });

    };

    /** END OF PUBLIC API **/

    return jsReviewForm;
});