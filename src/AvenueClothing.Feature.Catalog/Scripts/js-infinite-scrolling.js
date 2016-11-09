var InfiniteScrolling = (function () {
	var inScope = false;
	$(window).scroll(function () {
		if ($(window).scrollTop() + $(window).height() > $(document).height() - 200) {
			if (!inScope) {
				if ($(".category .product-list").length > 0) {
					var productsToSkip = $(".category .product-list .product").length;
					console.log("Number of already loaded products: " + productsToSkip);
				}
				inScope = true;
			}
		} else {
			inScope = false;
		}
	});
})();