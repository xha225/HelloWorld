
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
        protected static void WriteAt(string s, int x, int y, int sleep = 50)
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
            for (int i = 0; i < eatableChs.Count; i++)
            {
                if (SnakeCompareCoordinate(hwCoordinate[0], eatableChs[i].Value))
                {
                    // Add character to the end of the snake
                    snake = snake + eatableChs[i].Key;
                    // Add one coordinate, any value would do
                    hwCoordinate.Add(snakeSize, eatableChs[i].Value);
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

        // Print the outline of HELLO WORLD with any character
        protected static void PrintHelloWordOutline(char dot, string hw)
        {
            Dictionary<char, int[,]> bitImage = new Dictionary<char, int[,]>();
            bitImage.Add('h', new int[,]{
                          { 1, 0, 0, 1 },
                          { 1, 0, 0, 1 },
                          { 1, 1, 1, 1 },
                          { 1, 0, 0, 1 },
                          { 1, 0, 0, 1 }});

            bitImage.Add('e', new int[,]{
                          { 1, 1, 1, 1 },
                          { 1, 0, 0, 0 },
                          { 1, 1, 1, 1 },
                          { 1, 0, 0, 0 },
                          { 1, 1, 1, 1 }
                        });

            bitImage.Add('l', new int[,]{
                          { 1, 0, 0, 0 },
                          { 1, 0, 0, 0 },
                          { 1, 0, 0, 0 },
                          { 1, 0, 0, 0 },
                          { 1, 1, 1, 1 }
                        });

            bitImage.Add('o', new int[,]{
                          { 1, 1, 1, 1 },
                          { 1, 0, 0, 1 },
                          { 1, 0, 0, 1 },
                          { 1, 0, 0, 1 },
                          { 1, 1, 1, 1 }
                        });

            bitImage.Add('w', new int[,]{
                          { 1, 0, 0, 1 },
                          { 1, 1, 1, 1 },
                          { 1, 1, 1, 1 },
                          { 1, 1, 1, 1 },
                          { 1, 0, 0, 1 }
                        });

            bitImage.Add('r', new int[,]{
                          { 1, 1, 1, 1 },
                          { 1, 0, 0, 1 },
                          { 1, 1, 1, 1 },
                          { 1, 0, 1, 0 },
                          { 1, 0, 0, 1 }
                        });

            bitImage.Add('d', new int[,]{
                          { 1, 1, 1, 0 },
                          { 1, 0, 0, 1 },
                          { 1, 0, 0, 1 },
                          { 1, 0, 0, 1 },
                          { 1, 1, 1, 0 }
                        });

            bitImage.Add(' ', new int[,]{
                          { 0, 0, 0, 0 },
                          { 0, 0, 0, 0 },
                          { 0, 0, 0, 0 },
                          { 0, 0, 0, 0 },
                          { 0, 0, 0, 0 }
                        });

            bitImage.Add('!', new int[,]{
                          { 0, 1, 1, 0 },
                          { 0, 1, 1, 0 },
                          { 0, 1, 1, 0 },
                          { 0, 0, 0, 0 },
                          { 0, 1, 1, 0 }
                        });

            int dx = 0;

            foreach (char c in hw.ToLower())
            {
                for (int i = 0; i < bitImage[c].GetLength(0); i++)
                    
                    for (int j = 0; j < bitImage[c].GetLength(1); j++)
                    {
                        if(bitImage[c][i,j] == 1)
                            WriteAt(dot.ToString(), j + dx, i);
                    }
                dx += bitImage[c].GetLength(0);

            }

        }

        protected static void ChangeMatrixColor(int idx)
        {
            switch (idx)
            {
                case 0: // First color: White
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case 1: // 2nd - 4th color: Dark green
                case 2:
                case 3:
                case 4:                    
                case 5:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
                case 11: // 7th color: Black
                    Console.ForegroundColor = ConsoleColor.Black;
                    break;
            }
        }

        // The Matrix Effect
        protected static void TheMatrix()
        {
            
            List<KeyValuePair<char,int>> ms = new List<KeyValuePair<char,int>>();
            // Generate a random char and add to the top of the list
            int i;

            const int SIZE = 12;
            int dx = 10;
            // Bottom y coordinate
            int dy = 0;
            for(; ; )
            {
                // Clear console
                Console.Clear();
                // Check # of characters in the list
                if (ms.Count < SIZE)
                {
                    // Add a random character at the head of the list
                    Random r = new Random();
                    char c = (char)r.Next(32, 125);
                    ms.Insert(0, new KeyValuePair<char, int> (c, SIZE));

                    dy++;
                }

                //
                // Print characters and color
                for (i = 0; i < ms.Count; i++)
                {
                    ChangeMatrixColor(i);

                    // Update y coordinate
                    int y = dy - i;
                    WriteAt(ms[i].Key.ToString(), dx, y, 0);

                    // Decrease counter
                    if (ms[i].Value ==0)
                    {
                        // Remove from the list
                        ms.RemoveAt(i);
                    }
                    else { 
                    ms[i] = new KeyValuePair<char, int>(ms[i].Key,ms[i].Value-1);
                    }
                }
                Thread.Sleep(250);
                //WriteAt(c.ToString(), 1, 1, 300);

                // Grow the letter from 1 to 5
                // The bottom letter is in white
                // Change the bottom letter, and last three letters disappear after 3 moves
            }

        }

        // MAIN
        public static void Main()
        {
            string hw = "HELLO WORLD!";

            // TODO: Change mode before each run
            const int MODE = 6;

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
                case 5: // TODO: print hello world outline with characters
                    PrintHelloWordOutline('o',hw);
                    break;
                case 6: // TODO: The Matrix
                    TheMatrix();
                    break;
                case 7: // ? 
                    break;
                case 8: // Firework effect, characters going from bottom to top
                    break;
                case 9: // Greetings in random language
                    break;
                case 10: // Snake game-1, control "Hello World"
                    Snake(hw);
                    break;
                case 11: // Snake game-2, append characters
                    Snake2(hw);
                    break;
                case 12: // Snake game-3
                    break;
                case 13: // pac-man-1, hello world is the pac-man
                    break;
                case 14: // pac-man-2, use hello world to build the map
                    break;
                case 15: // Tetris game, move the character as it drops
                    break;
                case 16: // Play the hello world music
                    // https://khalidabuhakmeh.com/playing-the-super-mario-bros-theme-with-csharp
                    break;
                case 17: // ?
                    break;
                case 18:
                    break;
                case 19:
                    break;
                    /*case 19: 
                        break;
                    case 20: 
                        break;
                    case 21: 
                        break;
                    */
            }

        }
    }

}
