(function() {
	////////////////////////////////////////////////////////////////////////////////
	// ScriptProject.Program
	var $ScriptProject_Program = function() {
	};
	$ScriptProject_Program.$main = function() {
		var cursorX = 0, cursorY = 0;
		var holder = $('#main');
		var display = new ROT.Display({});
		$(function() {
			holder = $('#main').append(display.getContainer());
			display.draw(cursorX, cursorY, '@');
			//jQuery.Select("#main h2").Click(async (el, evt) =>
			//{
			//    await jQuery.Select("#main p").FadeOutTask();
			//    await jQuery.Select("#main p").FadeInTask();
			//    //Window.Alert("ROT call: " + ROT.isSupported());
			//});
		});
		document.addEventListener('keydown', function(e) {
			switch (e.keyCode) {
				case 37: {
					cursorX -= ((cursorX <= 0) ? 0 : 1);
					break;
				}
				case 38: {
					cursorY -= ((cursorY <= 0) ? 0 : 1);
					break;
				}
				case 39: {
					cursorX += ((cursorX > 80) ? 0 : 1);
					break;
				}
				case 40: {
					cursorY += ((cursorY > 25) ? 0 : 1);
					break;
				}
			}
			display.clear();
			display.draw(cursorX, cursorY, '@');
		});
		//
		//            jQuery.Select("*", display.getContainer()).Keydown((el, evt) =>
		//
		//            {
		//
		//            });
	};
	Type.registerClass(global, 'ScriptProject.Program', $ScriptProject_Program, Object);
	$ScriptProject_Program.$main();
})();
