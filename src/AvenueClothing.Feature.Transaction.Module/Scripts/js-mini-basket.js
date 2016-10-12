var MiniBasket = (function () {

    // declared with `var`, must be "private"
    var privateMethod = function () { };

    var publicScope = {
        init: function (rootSelector) {
            //$(rootSelector).find(".js-mini-basket")
        },
        basketChanged: function (rootSelector) {
            alert('TODO: Update mini basket');
        }
    };

    return publicScope;

})();