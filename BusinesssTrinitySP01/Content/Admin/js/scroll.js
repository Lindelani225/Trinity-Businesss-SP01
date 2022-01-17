(function () {

	function scroller() {

		var scroll = $('div.scroll');// Sets the div with a class of scroll as a variable

		var height = scroll.height(); // Gets the height of the scroll div

		var topAdj = -height - 100; /* '-height' turns the height of the UL into a negative #, 
               * '- 50' subtracts an extra 50 pixels from the height of 
        			 * the div so that it moves the trail of the UL higher to 
							 * the top of the div before the animation ends
							 */

		scroll.animate({
			'top': [topAdj, 'linear']
		}, 10000, function () {
			scroll.css('top', 0); //resets the top position of the Ul for the next cycle
			scroller(); // Recalls the animation cycle to begin.
		});
	}

	scroller();

})();


(function () {

	function textscroller() {

		var scrolltext = $('div.scroll-text');// Sets the div with a class of scroll as a variable

		var width = scrolltext.width(); // Gets the height of the scroll div

		var horizontalAdj = -width -120; /* '-height' turns the height of the UL into a negative #,
               * '- 50' subtracts an extra 50 pixels from the height of 
        			 * the div so that it moves the trail of the UL higher to 
							 * the top of the div before the animation ends
							 */

		scrolltext.animate({
			'left': [horizontalAdj, 'linear']
		}, 30000, function () {
			scrolltext.css('left', 1200); //resets the top position of the Ul for the next cycle
			textscroller(); // Recalls the animation cycle to begin.
		});
	}

	textscroller();

})();