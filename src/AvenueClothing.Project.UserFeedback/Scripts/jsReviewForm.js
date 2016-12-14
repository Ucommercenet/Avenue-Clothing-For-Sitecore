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
                $('label', radios).each(function () {
                    var t = $(this);
                    setStarHoverOutState($('i', t));
                    t.removeClass('selected');
                    if (t.hasClass('selected')) {
                        setStarHoverState($('i', t));
                    }
                });
                parent.addClass('selected');
                $('#review-form').slideDown();
                $('label', radios).each(function () {
                    var t = $(this);
                    if (t.hasClass('selected')) {
                        setStarHoverState($('i', t.prevAll('label')));
                        setStarHoverState($('i', t));
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

    // var validated= function validateForm() {
    //     $reviewForm.each(function () {

    //         $reviewForm.validate({
    //             errorElement: "span",
    //             errorClass: "help-inline",
    //             highlight: function(tag) {
    //                 $(tag).closest('.control-tag').addClass('error');
    //                 return false;
    //             },
    //             success: function(tag) {
    //                 tag.closest('.control-tag').addClass('success');
    //             }
    //         });
    //     });
    //     return true;
    //     };

    jsReviewForm.init = function () {
        wireupRatings(config.$rootSelector.find(classSelector));


        var submit = $reviewForm.data('submit-button');

        $(submit).off().click(function (e) {
            e.preventDefault();
            var serializedFormData = $reviewForm.serializeArray();
            var values = {};

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
            // $reviewForm.load(location.href + ' .review-form');
            $reviewForm[0].reset();
        });

    };

    /** END OF PUBLIC API **/

    return jsReviewForm;
});