using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Html;
using jQueryApi;
using System.Threading.Tasks;
using ROT;
namespace Forays
{
    public class ConsoleKey
    {
        public const int D0 = 48;
        public const int D1 = 49;
        public const int D2 = 50;
        public const int D3 = 51;
        public const int D4 = 52;
        public const int D5 = 53;
        public const int D6 = 54;
        public const int D7 = 55;
        public const int D8 = 56;
        public const int D9 = 57;
        public const int Alt = 18;
        public const int Backspace = 8;
        public const int CAPS_LOCK = 20;
        public const int COMMA = 188;
        public const int COMMAND = 91;
        public const int COMMAND_LEFT = 91;
        public const int COMMAND_RIGHT = 93;
        public const int CONTROL = 17;
        public const int Delete = 46;
        public const int DownArrow = 40;
        public const int End = 35;
        public const int Enter = 13;
        public const int Escape = 27;
        public const int Home = 36;
        public const int INSERT = 45;
        public const int LeftArrow = 37;
        public const int MENU = 93;
        public const int NUMPAD_ADD = 107;
        public const int NUMPAD_DECIMAL = 110;
        public const int NUMPAD_DIVIDE = 111;
        public const int NUMPAD_ENTER = 108;
        public const int NUMPAD_MULTIPLY = 106;
        public const int NUMPAD_SUBTRACT = 109;
        public const int PageDown = 34;
        public const int PageUp = 33;
        public const int PERIOD = 190;
        public const int RightArrow = 39;
        public const int SHIFT = 16;
        public const int SPACE = 32;
        public const int Tab = 9;
        public const int UpArrow = 38;
        public const int WINDOWS = 91;
        public const int NumPad0 = 96;
        public const int NumPad1 = 97;
        public const int NumPad2 = 98;
        public const int NumPad3 = 99;
        public const int NumPad4 = 100;
        public const int NumPad5 = 101;
        public const int NumPad6 = 102;
        public const int NumPad7 = 103;
        public const int NumPad8 = 104;
        public const int NumPad9 = 105;

    }
    public enum ConsoleColor
    {
        Black, DarkBlue, DarkGreen, DarkCyan, DarkRed, DarkMagenta, DarkYellow, Gray, DarkGray, Blue, Green, Cyan, Red, Magenta, Yellow, White
    }
    public static class ConsoleModifiers
    {
        public static int Plain = 0; public static int Alt = 1;
        public static int Shift = 2; public static int Control = 4;
    }
    public class ConsoleKeyInfo
    {
        public int Key;
        public char KeyChar;
        public int Modifiers;
        public ConsoleKeyInfo(int key)
        {
            Key = key;
            KeyChar = ((char)key).ToString().ToLower()[0];
            Modifiers = ConsoleModifiers.Plain;
        }

        public ConsoleKeyInfo(int key, int mods)
        {
            Key = key;
            if((mods & ConsoleModifiers.Shift) == ConsoleModifiers.Shift)
                KeyChar = ((char)key).ToString().ToUpper()[0];
            else
                KeyChar = ((char)key).ToString().ToLower()[0];
            Modifiers = mods;
        }
        public ConsoleKeyInfo(char keycode)
        {
            Key = (int)keycode;
            KeyChar = keycode;
            Modifiers = ConsoleModifiers.Plain;
        }
    }

    public class ROTConsole
    {
        public Display display;
        public int CursorLeft = 0;
        public int CursorTop = 0;
        public bool CursorVisible = false;
        private string bg, fg;
        public ConsoleColor Background = ConsoleColor.Black;
        public ConsoleColor BackgroundColor
        {
            get { return Background; }
            set
            {
                Background = value;
                assignBG(value);
            }
        }

        private void assignBG(ConsoleColor value)
        {
            switch (value)
            {
                case ConsoleColor.Black: bg = "#000000"; break;
                case ConsoleColor.Blue: bg = "#0000FF"; break;
                case ConsoleColor.Cyan: bg = "#00FFFF"; break;
                case ConsoleColor.DarkBlue: bg = "#000099"; break;
                case ConsoleColor.DarkCyan: bg = "#009999"; break;
                case ConsoleColor.DarkGray: bg = "#666666"; break;
                case ConsoleColor.DarkGreen: bg = "#009900"; break;
                case ConsoleColor.DarkMagenta: bg = "#990099"; break;
                case ConsoleColor.DarkRed: bg = "#990000"; break;
                case ConsoleColor.DarkYellow: bg = "#999900"; break;
                case ConsoleColor.Gray: bg = "#AAAAAA"; break;
                case ConsoleColor.Green: bg = "#00FF00"; break;
                case ConsoleColor.Magenta: bg = "#FF00FF"; break;
                case ConsoleColor.Red: bg = "#FF0000"; break;
                case ConsoleColor.White: bg = "#FFFFFF"; break;
                case ConsoleColor.Yellow: bg = "#FFFF00"; break;
            }
        }
        private void assignFG(ConsoleColor value)
        {
            switch (value)
            {
                case ConsoleColor.Black: fg = "#000000"; break;
                case ConsoleColor.Blue: fg = "#0000FF"; break;
                case ConsoleColor.Cyan: fg = "#00FFFF"; break;
                case ConsoleColor.DarkBlue: fg = "#000099"; break;
                case ConsoleColor.DarkCyan: fg = "#009999"; break;
                case ConsoleColor.DarkGray: fg = "#666666"; break;
                case ConsoleColor.DarkGreen: fg = "#009900"; break;
                case ConsoleColor.DarkMagenta: fg = "#990099"; break;
                case ConsoleColor.DarkRed: fg = "#990000"; break;
                case ConsoleColor.DarkYellow: fg = "#999900"; break;
                case ConsoleColor.Gray: fg = "#AAAAAA"; break;
                case ConsoleColor.Green: fg = "#00FF00"; break;
                case ConsoleColor.Magenta: fg = "#FF00FF"; break;
                case ConsoleColor.Red: fg = "#FF0000"; break;
                case ConsoleColor.White: fg = "#FFFFFF"; break;
                case ConsoleColor.Yellow: fg = "#FFFF00"; break;
            }
        }
        public ConsoleColor Foreground = ConsoleColor.White;
        public ConsoleColor ForegroundColor
        {
            get { return Foreground; }
            set
            {
                Foreground = value;
                assignFG(value);
            }
        }
        public bool KeyAvailable = false;
        private bool Intercept = false;
        private ConsoleKeyInfo kc = new ConsoleKeyInfo(65);
        //private Element keydiv;
        public ROTConsole()
        {
            display = new Display(new DisplayOptions(80, 25));
        }
        private TaskCompletionSource<ConsoleKeyInfo> defr;// Task<ConsoleKeyInfo> cki = null;
        private async void processKey(Element elem, jQueryEvent ev)
        {
            int m = 0;
            if (ev.AltKey) m = m | ConsoleModifiers.Alt;
            if (ev.CtrlKey) m = m | ConsoleModifiers.Control;
            if (ev.ShiftKey) m = m | ConsoleModifiers.Shift;
            if (m != 0)
                kc = new ConsoleKeyInfo(ev.Which, m);
            else
                kc = new ConsoleKeyInfo(ev.Which);
            /*if (!Intercept && KeyAvailable)
            {
               // SetCursorPosition(CursorLeft - 1, CursorTop);
                Write(kc.KeyChar);
            }*/
            jQuery.Select("#key").ReplaceWith("<div id=\"key\"><p>Key Down, Key is " + kc.Key + ", Char is " + kc.KeyChar.ToString() + "</p></div>");
            //cki = Task<ConsoleKeyInfo>.FromResult(kc);
            KeyAvailable = false;
            //jQuery.Select("#main").Off("keyup", "canvas", processKey);
            //defr = new TaskCompletionSource<ConsoleKeyInfo>();
            defr.TrySetResult(kc);
            await Task.Delay(10);
        }
        /*        public async Task<ConsoleKeyInfo> ReadKey()
                {
                    await Task.Run(() => (jQuery.Select("body").On("keydown", processKey)));
                    jQuery.Select("body").Off("keydown", "body", processKey);
                    return kc;

                }*/
        public async Task<ConsoleKeyInfo> ReadKey(bool intercept)
        {
//            cki = null;
            Intercept = intercept;
            defr = new TaskCompletionSource<ConsoleKeyInfo>();
            //defr.Done(() => );
            jQuery.Select("body").One("keyup", processKey);
            //(, 2, "on", "keydown", "canvas", "processKey");
           /*while (cki == null)
               await Task.Delay(35);*/
            kc = await defr.Task;
            return kc;
        }
        public void SetCursorPosition(int x, int y)
        {
            CursorLeft = x;
            CursorTop = y;
        }
        public void Write(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                display.draw(CursorLeft+i, CursorTop, text[i].ToString(), fg, bg);
            }
        }
        public void Write(char text)
        {
            display.draw(CursorLeft, CursorTop, (string)text, fg, bg);
        }
    }
}
