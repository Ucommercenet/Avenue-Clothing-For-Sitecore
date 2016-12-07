define('jsAddReview', ['jquery', 'jsConfig'], function ($, config) {
    'use strict';

    var classSelector = '.js-add-review';


    var reviewAdded = function (event, data) {
        var $reviewList = event.data.$element;

        var newReview =
        '<section itemprop="review" itemscope itemtype="http://schema.org/Review" class="review">' +
        '<header>' +
        '<div itemprop="reviewRating" itemscope itemtype="http://schema.org/Rating" class="review-stars">' +
        '<span class="star-rating">';

        for (var i = 20; i <= 100; i = i + 20) {
            if (data.Rating >= i) {
                newReview = newReview + '<i class="fa fa-star"></i>';
            } else {
                newReview = newReview + '<i class="fa fa-star-o"></i>';
            }

        }

        newReview = newReview +
        '</span>' +
        '</div>' +
         '<p itemprop="name" class="review-headline">' + data.ReviewHeadline + '</p>' +
        '</header>' +
        '<aside class="review-by">' +
        '<p>' +
        'by <span itemprop="author">' + data.CreatedBy + '</span> on ' + data.CreatedOn +
        '</p> </aside>' +
        '<p itemprop="description">' + data.Comments + '</p>' +
        '<meta itemprop="ratingValue" content="' + data.Rating + '">' +
        '<meta itemprop="worstRating content="1"' +
        '<meta itemprop="bestRating" content="5"' +
        '<meta itemprop="datePublished" content="' + data.CreatedOnForMeta + '">' +
        '</section>';

        $reviewList.append(newReview);

    }

    var jsAddReview = {};

    jsAddReview.init = function () {
        config.$rootSelector.find(classSelector).each(function () {
            config.$triggerEventSelector.on("review-added", { $element: $(this) }, reviewAdded);
        })
    };

    return jsAddReview;
});