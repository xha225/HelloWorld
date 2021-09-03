
using System;
using System.Collections.Generic;
using System.Threading;
using static System.Console;

namespace CS258
{
    class Program
    {
        protected static int origRow;
        protected static int origCol;

        // https://docs.microsoft.com/en-us/dotnet/api/system.console.setcursorposition?view=net-5.0
        protected static void WriteAt(string s, int x, int y, int sleep = 0)
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
            Console.ForegroundColor = ConsoleColor.Green;

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
                    WriteAt(hw[i].ToString(), j, 2, 300);

                    if (j != i + 1)
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
                for (int j = TOP; j < BOTTOM - i; j++)
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
                for (int j = BOTTOM; j > i - TOP; j--)
                {
                    WriteAt(hw[i].ToString(), X_AXIS, j, 100);

                    if (j != i - TOP + 1)
                        WriteAt(" ", X_AXIS, j, 1);

                }
            }
        }

        protected static void WriteAtCoordinate(Dictionary<int, int[]> coordinate, string s)
        {
            foreach (var pair in coordinate)
            {

                WriteAt(s[pair.Key].ToString(), pair.Value[0], pair.Value[1]);
            }
        }
        protected static void UpdateSnakeCoordinates(int x, int y, Dictionary<int, int[]> coordinate, string s)
        {
            for (int i = s.Length - 1; i > 0; i--)
            {
                // Update character coordinate except the first character
                coordinate[i] = coordinate[i - 1];
            }

            // Update head
            coordinate[0] = new int[] { x, y };
        }

        protected static void DrawBorder(int height, int width)
        {
            // left
            WriteAt("*", 0, 0);

            int i;

            for (i = 1; i < height; i++)
            {
                WriteAt("|", 0, i);
            }
            WriteAt("*", 0, i);
            int yAxis = i;

            // bottom
            for (i = 1; i < width; i++)
            {
                WriteAt("-", i, yAxis);
            }
            WriteAt("*", i, yAxis);
            int xAxis = i;

            // right
            for (i = yAxis - 1; i > 0; i--)
            {
                WriteAt("|", xAxis, i);
            }
            WriteAt("*", xAxis, 0);

            // top
            for (i = width - 1; i > 0; i--)
            {
                WriteAt("-", i, 0);
            }
        }

        // The Snake Game V1
        protected static void Snake(string hw)
        {
            // Reference: https://jbwyatt.com/Code/Csharp/code/snake.cs.html

            bool gameLive = true;
            ConsoleKeyInfo consoleKey;

            // location info & display
            int x = 0, y = 2; // y is 2 to allow the top row for directions & space
            int dx = 1, dy = 0;
            const int CONSOLE_WIDTH = 79;
            const int CONSOLE_HEIGHT = 24;
            Dictionary<int, int[]> hwCoordinate = new Dictionary<int, int[]>();

            int i;
            // Initialize character coordinates
            for (i = 0; i < hw.Length; i++)
            {
                int[] coordinate = { i, y };
                hwCoordinate.Add(i, coordinate);
            }

            WriteAtCoordinate(hwCoordinate, hw);

            // print directions at top, then restore position
            // save then restore current color
            Console.ForegroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Snake - Hello World");
            Console.WriteLine("Arrows move up/down/right/left. Press 'esc' quit.");
            //TODO: draw the border
            DrawBorder(CONSOLE_HEIGHT, CONSOLE_WIDTH);

            do // until escape
            {
                // see if a key has been pressed
                if (Console.KeyAvailable)
                {
                    // get key and use it to set options
                    consoleKey = Console.ReadKey(true);
                    switch (consoleKey.Key)
                    {
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
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        case ConsoleKey.Escape: //END
                            gameLive = false;
                            break;
                    }

                    // calculate the new position
                    // note x set to 0 because we use the whole width, but y set to 1 because we use top row for instructions
                    x += dx;
                    if (x > CONSOLE_WIDTH)
                        x = 0;
                    if (x < 0)
                        x = CONSOLE_WIDTH;

                    y += dy;
                    if (y > CONSOLE_HEIGHT)
                        y = 0;
                    if (y < 0)
                        y = CONSOLE_HEIGHT;

                    // Clear screen
                    // TODO: clear only a portion of the screen
                    Console.Clear();
                    // Re-Draw border
                    DrawBorder(CONSOLE_HEIGHT, CONSOLE_WIDTH);
                    // write the character in the new position
                    Console.SetCursorPosition(x, y);
                    // Update character coordinates
                    UpdateSnakeCoordinates(x, y, hwCoordinate, hw);
                    WriteAtCoordinate(hwCoordinate, hw);
                    //Console.Write(ch);
                }

            } while (gameLive);

        }

        protected static void PrintEatableCharacters(List<KeyValuePair<char, int[]>> chs)
        {
            foreach (KeyValuePair<char, int[]> k in chs)
            {
                WriteAt(k.Key.ToString(), k.Value[0], k.Value[1]);
            }
        }

        protected static bool SnakeCompareCoordinate(int[] c1, int[] c2)
        {
            if (c1[0] == c2[0] && c1[1] == c2[1])
                return true;
            else
                return false;
        }

        protected static void SankeContact(Dictionary<int, int[]> hwCoordinate,
            List<KeyValuePair<char, int[]>> eatableChs, ref string snake)
        {
            // Get snake size
            int snakeSize = hwCoordinate.Count;
            for (int i=0;i<eatableChs.Count;i++)
            {
                if(SnakeCompareCoordinate(hwCoordinate[0], eatableChs[i].Value))
                {
                    // Add character to the end of the snake
                    snake = snake + eatableChs[i].Key;
                    // Add one coordinate, any value would do
                    hwCoordinate.Add(snakeSize,eatableChs[i].Value);
                    // Remove eatable
                    eatableChs.RemoveAt(i);
                }
            }            
        }

        // The Snake Game V2, add random eatable characters
        protected static void Snake2(string hw)
        {
            // Reference: https://jbwyatt.com/Code/Csharp/code/snake.cs.html

            bool gameLive = true;
            ConsoleKeyInfo consoleKey;

            // Counter
            int i;
            // location info & display
            int x = 0, y = 2; // y is 2 to allow the top row for directions & space
            int dx = 1, dy = 0;
            const int CONSOLE_WIDTH = 79;
            const int CONSOLE_HEIGHT = 24;
            Dictionary<int, int[]> hwCoordinate = new Dictionary<int, int[]>();
            List<KeyValuePair<char, int[]>> eatableChs = new List<KeyValuePair<char, int[]>>();

            const int RANDOM_CH_COUNT = 10;
            Random r = new Random();

            for (i = 0; i < RANDOM_CH_COUNT; i++)
            {
                KeyValuePair<char, int[]> kv =
                new KeyValuePair<char, int[]>((char)r.Next(65, 90),
                                                new int[] { r.Next(1, CONSOLE_WIDTH),
                                                r.Next(1,CONSOLE_HEIGHT)});

                eatableChs.Add(kv);
            }

            // Print eatable characters
            PrintEatableCharacters(eatableChs);

            // Initialize character coordinates
            for (i = 0; i < hw.Length; i++)
            {
                int[] coordinate = { i, y };
                hwCoordinate.Add(i, coordinate);
            }

            WriteAtCoordinate(hwCoordinate, hw);

            // print directions at top, then restore position
            // save then restore current color
            Console.ForegroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Snake - Hello World");
            Console.WriteLine("Arrows move up/down/right/left. Press 'esc' quit.");

            // Draw the border
            DrawBorder(CONSOLE_HEIGHT, CONSOLE_WIDTH);

            // Draw random characters

            do // until escape
            {
                // see if a key has been pressed
                if (Console.KeyAvailable)
                {
                    // get key and use it to set options
                    consoleKey = Console.ReadKey(true);
                    switch (consoleKey.Key)
                    {
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
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        case ConsoleKey.Escape: //END
                            gameLive = false;
                            break;
                    }

                    // calculate the new position
                    // note x set to 0 because we use the whole width, but y set to 1 because we use top row for instructions
                    x += dx;
                    if (x > CONSOLE_WIDTH)
                        x = 0;
                    if (x < 0)
                        x = CONSOLE_WIDTH;

                    y += dy;
                    if (y > CONSOLE_HEIGHT)
                        y = 0;
                    if (y < 0)
                        y = CONSOLE_HEIGHT;

                    // Clear screen
                    // TODO: clear only a portion of the screen
                    Console.Clear();
                    // Re-Draw border
                    DrawBorder(CONSOLE_HEIGHT, CONSOLE_WIDTH);

                    // Write the character in the new position
                    //Console.SetCursorPosition(x, y);
                    // Update character coordinates
                    UpdateSnakeCoordinates(x, y, hwCoordinate, hw);
                    WriteAtCoordinate(hwCoordinate, hw);
                    
                    // TODO: check for contact
                    SankeContact(hwCoordinate, eatableChs, ref hw);

                    // Print eatable characters
                    PrintEatableCharacters(eatableChs);

                    // Print Contratulations once all eatable characters are gone
                    if (eatableChs.Count == 0)
                    {
                        Console.Clear();
                        Console.WriteLine("Congratulations");
                        gameLive = false;
                    }
                }
            } while (gameLive);

        }
        // MAIN
        public static void Main()
        {
            string hw = "HELLO WORLD!";

            // TODO: Change mode before each run
            const int MODE = 18;

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
                case 17: // Snake game-1, control "Hello World"
                    Snake(hw);
                    break;
                case 18: // Snake game-2, append characters
                    Snake2(hw);
                    break;
                case 19: // pac-man-1, hello world is the pac-man
                    break;
                case 20: // pac-man-2, use hello world to build the map
                    break;
                case 21: // Play the hello world music
                    // https://khalidabuhakmeh.com/playing-the-super-mario-bros-theme-with-csharp
                    break;

            }

        }
    }

}
