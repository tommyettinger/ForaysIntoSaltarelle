using System;
using System.Collections.Generic;
using System.Html;
using jQueryApi;
using ROT;

namespace ScriptProject
{
    public class Program
    {

        static void Main()
        {
            int cursorX = 0, cursorY = 0;
            jQueryObject holder = jQuery.Select("#main");
            Display display = new Display(new DisplayOptions());
            jQuery.OnDocumentReady(() => 
            {
                holder = jQuery.Select("#main").Append(display.getContainer());
                display.draw(cursorX, cursorY, "@");

                //jQuery.Select("#main h2").Click(async (el, evt) =>
                //{
                //    await jQuery.Select("#main p").FadeOutTask();
                //    await jQuery.Select("#main p").FadeInTask();

                //    //Window.Alert("ROT call: " + ROT.isSupported());
                //});
            });
            Document.AddEventListener("keydown", (e) =>
            {
                switch (e.KeyCode)
                {
                    case 37: cursorX -= (cursorX <= 0) ? 0 : 1;
                        break;
                    case 38: cursorY -= (cursorY <= 0) ? 0 : 1;
                        break;
                    case 39: cursorX += (cursorX > 80) ? 0 : 1;
                        break;
                    case 40: cursorY += (cursorY > 25) ? 0 : 1;
                        break;
                }
                display.clear();
                display.draw(cursorX, cursorY, "@");
            });
            /*
            jQuery.Select("*", display.getContainer()).Keydown((el, evt) =>
            {
            });*/
        }
    }
}