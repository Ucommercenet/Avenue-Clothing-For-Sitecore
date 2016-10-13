$(function () {
	var inScope = false;
	var currentAmountOfProducts = 25;
	$(window).scroll(function () {
		if ($(window).scrollTop() + $(window).height() > $(document).height() - 200 && !inScope) {
			console.log("bottom!");
			inScope = true;
		} else if ($(window).scrollTop() + $(window).height() < $(document).height() - 200) {
			inScope = false;
		}
	});
});
