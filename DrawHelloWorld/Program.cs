
using System;
using System.Threading;
using static System.Console;

namespace CS258
{
    class Program
    {
        protected static int origRow;
        protected static int origCol;

        // https://docs.microsoft.com/en-us/dotnet/api/system.console.setcursorposition?view=net-5.0
        protected static void WriteAt(string s, int x, int y, int sleep=100)
        {
            try
            {
                Console.SetCursorPosition(origCol + x, origRow + y);
                Console.Write(s);
                Thread.Sleep(sleep);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.Clear();
                Console.WriteLine(e.Message);
            }
        }

        protected static void PrintBox(string hw)
        {
            // Clear the screen, then save the top and left coordinates.
            Console.Clear();
            origRow = Console.CursorTop;
            origCol = Console.CursorLeft;

            // Draw the left side of a 5x5 rectangle, from top to bottom.
            WriteAt("+", 0, 0);
            WriteAt("|", 0, 1);
            WriteAt("|", 0, 2);
            WriteAt("|", 0, 3);
            WriteAt("+", 0, 4);

            // Draw the bottom side, from left to right.
            WriteAt("-", 1, 4); // shortcut: WriteAt("---", 1, 4)
            int i = 0;
            for (; i <= hw.Length + 1; i++)
                WriteAt("-", i + 1, 4); // ...
            int rb = i + 1;
            WriteAt("+", rb, 4);


            // Draw the right side, from bottom to top.
            WriteAt("|", rb, 3);
            WriteAt("|", rb, 2);
            WriteAt("|", rb, 1);
            WriteAt("+", rb, 0);

            // Draw the top side, from right to left.
            for (i = hw.Length + 2; i > 0; i--)
                WriteAt("-", i, 0); // ...
        }

        protected static void PrintLeftToRight(string hw)
        {

            PrintBox(hw);
            // Change font color
            //Console.ForegroundColor = ConsoleColor.Green;

            Random r = new Random();
            int range = Enum.GetValues(typeof(ConsoleColor)).Length;

            // Print Hello World

            for (int i = 0; i < hw.Length; i++)
            {

                Console.ForegroundColor = (ConsoleColor)r.Next(0, range);
                WriteAt(hw[i].ToString(), i + 2, 2);
            }

        }

        protected static void PrintRightToLeft(string hw)
        {

            PrintBox(hw);
            // Change font color
            //Console.ForegroundColor = ConsoleColor.Green;

            Random r = new Random();
            int range = Enum.GetValues(typeof(ConsoleColor)).Length;

            // Animated HELLO WORLD
            for (int i = 0; i < hw.Length; i++)
            {

                // Console.ForegroundColor = (ConsoleColor)r.Next(0, range); 
                // WriteAt(hw[i].ToString(), i + 2, 2);

                // Flowing from right to left
                for (int j = hw.Length + 1; j > i; j--)
                {
                    WriteAt(hw[i].ToString(), j, 2,300);
                    
                    if (j!=i+1)
                        WriteAt(" ", j, 2, 1);
                }
            }

        }


        // MODE = 3
        protected static void PrintTopToBottom(string hw)
        {

            //PrintBox(hw);
            
            Random r = new Random();
            int range = Enum.GetValues(typeof(ConsoleColor)).Length;

            const int TOP = 0;
            const int BOTTOM = 15;
            const int X_AXIS = 10;
            
            for (int i = 0; i < hw.Length; i++)
            {

                Console.ForegroundColor = (ConsoleColor)r.Next(0, range); 
                

                // Flowing from top to bottom
                for (int j = TOP; j < BOTTOM-i; j++)
                {
                    WriteAt(hw[i].ToString(), X_AXIS, j, 100);

                    if (j != BOTTOM - i - 1)
                        WriteAt(" ", X_AXIS, j, 1);
                    
                }
            }
        }

        // MODE = 4
        protected static void PrintBottomToTop(string hw)
        {

            Random r = new Random();
            int range = Enum.GetValues(typeof(ConsoleColor)).Length;

            const int TOP = 0;
            const int BOTTOM = 15;
            const int X_AXIS = 10;

            for (int i = 0; i < hw.Length; i++)
            {

                Console.ForegroundColor = (ConsoleColor)r.Next(0, range);


                // Flowing from bottom to top
                for (int j = BOTTOM; j > i-TOP; j--)
                {
                    WriteAt(hw[i].ToString(), X_AXIS, j, 100);

                    if (j != i-TOP+1)
                        WriteAt(" ", X_AXIS, j, 1);

                }
            }
        }

        // Snake
        protected static void Snake()
        {
            // Reference: https://jbwyatt.com/Code/Csharp/code/snake.cs.html
            // start game
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            // display this char on the console during the game
            char ch = '*';
            bool gameLive = true;
            ConsoleKeyInfo consoleKey; // holds whatever key is pressed

            // location info & display
            int x = 0, y = 2; // y is 2 to allow the top row for directions & space
            int dx = 1, dy = 0;
            int consoleWidthLimit = 79;
            int consoleHeightLimit = 24;

            // clear to color
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.Clear();

            // delay to slow down the character movement so you can see it
            int delayInMillisecs = 50;

            // whether to keep trails
            bool trail = true;

            do // until escape
            {
                // print directions at top, then restore position
                // save then restore current color
                ConsoleColor cc = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("Arrows move up/down/right/left. 't' trail.  'c' clear  'esc' quit.");
                Console.SetCursorPosition(x, y);
                Console.ForegroundColor = cc;

                // see if a key has been pressed
                if (Console.KeyAvailable)
                {
                    // get key and use it to set options
                    consoleKey = Console.ReadKey(true);
                    switch (consoleKey.Key)
                    {
                        case ConsoleKey.T:
                            trail = true;
                            break;
                        case ConsoleKey.C:
                            Console.BackgroundColor = ConsoleColor.DarkGray;
                            trail = true;
                            Console.Clear();
                            break;
                        case ConsoleKey.UpArrow: //UP
                            dx = 0;
                            dy = -1;
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;
                        case ConsoleKey.DownArrow: // DOWN
                            dx = 0;
                            dy = 1;
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            break;
                        case ConsoleKey.LeftArrow: //LEFT
                            dx = -1;
                            dy = 0;
                            Console.ForegroundColor = ConsoleColor.Green;
                            break;
                        case ConsoleKey.RightArrow: //RIGHT
                            dx = 1;
                            dy = 0;
                            Console.ForegroundColor = ConsoleColor.Black;
                            break;
                        case ConsoleKey.Escape: //END
                            gameLive = false;
                            break;
                    }
                }

                // find the current position in the console grid & erase the character there if don't want to see the trail
                Console.SetCursorPosition(x, y);
                if (trail == false)
                    Console.Write(' ');

                // calculate the new position
                // note x set to 0 because we use the whole width, but y set to 1 because we use top row for instructions
                x += dx;
                if (x > consoleWidthLimit)
                    x = 0;
                if (x < 0)
                    x = consoleWidthLimit;

                y += dy;
                if (y > consoleHeightLimit)
                    y = 2; // 2 due to top spaces used for directions
                if (y < 2)
                    y = consoleHeightLimit;

                // write the character in the new position
                Console.SetCursorPosition(x, y);
                Console.Write(ch);

                // pause to allow eyeballs to keep up
                System.Threading.Thread.Sleep(delayInMillisecs);

            } while (gameLive);
        }

        // MAIN
        public static void Main()
        {
            string hw = "HELLO WORLD!";

            // TODO: Change mode before each run
            const int MODE = 1;

            switch (MODE)
            {
                case 0: // print hello world;
                    Console.WriteLine(hw);
                    break;
                case 1: // print from left to right
                    PrintLeftToRight(hw);
                    break;
                case 2: // print from right to left
                    PrintRightToLeft(hw);
                    break;
                case 3: // print from top to bottom
                    char[] reversedString = hw.ToCharArray();
                    Array.Reverse(reversedString);
                    hw = new string(reversedString);
                    PrintTopToBottom(hw);
                    break;
                case 4: // print from bottom to top
                    PrintBottomToTop(hw);
                    break;
                case 5: // sine
                    break;
                case 6:// rotating characters
                    break;
                case 7:// print hello world outline with character a
                    break;
                case 8: // star war effect
                    break;
                case 9: // the matrix effect
                    break;
                case 10: // spiral effect
                    break; //
                case 11: // jet effect
                    break;
                case 12: // change font typeface and size
                    break;
                case 13: // heart pulse effect
                    break;
                case 14: // print with sound effect
                    break;
                case 15: // Tetris game, move the character as it drops
                    break;
                case 16: // Firework effect, characters going from bottom to top
                    break;
                case 17: // Snake game, control "Hello World"
                    Snake();
                    break;
            }
                
            
            

            

        }
    }

}
