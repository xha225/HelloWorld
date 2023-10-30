using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using static CS258.Program;
namespace CS258
{
    public class Tetris
    {
        public class TetrisChar
        {
            private char _ch;
            public char ChValue
            {
                get { return _ch; }
            }

            private ConsoleColor _color;
            public ConsoleColor ChColor
            {
                get { return _color; }
            }

            private Point _pos;
            public Point Pos
            {
                get { return _pos; }
                set { _pos = value; }
            }

            public TetrisChar(char c, ConsoleColor cc, Point p)
            {
                _ch = c;
                _color = cc;
                _pos = p;
            }
        }

        private const int STACKS = 25;
        private const int DROP_POINT = 1;
        private const int MAX_STACK_HEIGHT = 5;

        protected static void DrawTetrisStack(List<List<TetrisChar>> l)
        {
            Console.Clear();
            for (int i = 0; i < l.Count; i++)
            {
                foreach (TetrisChar tc in l[i])
                {
                    Console.ForegroundColor = tc.ChColor;
                    WriteAt(tc.ChValue.ToString(), tc.Pos.X, tc.Pos.Y, 0);
                }
            }
        }

        protected static List<TetrisChar> CreateTetrisStack(int height, int columnIdx)
        {
            // Generate columns based on column index and height
            Random r = new Random();
            int i = r.Next(0, _HW.Length);
            int colorRange = Enum.GetValues(typeof(ConsoleColor)).Length;

            List<TetrisChar> l = new List<TetrisChar>();
            int localHeight = r.Next(height, CONSOLE_HEIGHT);

            for (i = CONSOLE_HEIGHT; i > localHeight; i--)
            {
                int j = r.Next(0, _HW.Length);
                // Replace empty string
                if (_HW[j].ToString() == " ")
                {
                    j++;
                }

                TetrisChar tc = new TetrisChar(_HW[j], (ConsoleColor)r.Next(1, colorRange), new Point(columnIdx, i));
                l.Add(tc);
            }

            return l;
        }

        protected static TetrisChar ReturnTetrisBlock(int numOfStacks, int y)
        {

            Random r = new Random();
            int range = Enum.GetValues(typeof(ConsoleColor)).Length;
            int idx = r.Next(0, _HW.Length);
            if (_HW[idx].ToString() == " ")
            {
                idx++;
            }
            char c = _HW[idx];
            int x = r.Next(0, numOfStacks);
            // TODO: update y axis in Point
            return new TetrisChar(c, (ConsoleColor)r.Next(0, range), new Point(x, y));
        }

        protected static bool IsSameTetrisBlock(TetrisChar tc1, TetrisChar tc2)
        {
            if (tc1.ChValue == tc2.ChValue)// || tc1.ChColor == tc2.ChColor)
                return true;

            return false;
        }

        protected static bool IsTetrisBlockLanded(TetrisChar tc, List<List<TetrisChar>> stacks)
        {
            // Check only for a given stack by the x axis, and check only with the stack top
            int stackIdx = tc.Pos.X;
            int topCharIdx = stacks[stackIdx].Count - 1;
            int lastStackIdx = stacks.Count - 1;
            bool remove = false;
            bool rightmostStack = false;
            bool leftmostStack = false;
            bool hitBottom = false;


            // when thre is no blocks in the stack
            if (topCharIdx == -1)
            {
                // Add block to stack right before hitting the bottom
                if (tc.Pos.Y == CONSOLE_HEIGHT - 1)
                {
                    TetrisChar tcBlock = new TetrisChar(tc.ChValue, tc.ChColor, new Point(tc.Pos.X, CONSOLE_HEIGHT));
                    stacks[stackIdx].Add(tcBlock);

                    hitBottom = true;
                }
                else
                {
                    return false;
                }
            }

            // Empty Stack
            if (topCharIdx == -1 && hitBottom)
            {
                int idx = 0; // bottom
                // Check with left stack
                if (stackIdx == 0)
                {
                    leftmostStack = true;
                }

                if (stackIdx == lastStackIdx)
                {
                    rightmostStack = true;
                }

                // if not leftmost stack, check left stack
                if (!leftmostStack)
                {
                    int leftStackIdx = stackIdx - 1;
                    if (stacks[leftStackIdx].Count > idx)
                    {
                        if (IsSameTetrisBlock(tc, stacks[leftStackIdx][idx]))
                        {
                            stacks[leftStackIdx].RemoveAt(idx);
                            stacks[stackIdx].RemoveAt(idx);
                            // TODO: update score
                        }
                    }
                }

                // If not rightmost stack, check right stack
                if (!rightmostStack)
                {
                    int nextStackIdx = stackIdx + 1;
                    if (stacks[nextStackIdx].Count > idx)
                    {
                        if (IsSameTetrisBlock(tc, stacks[nextStackIdx][idx]))
                        {
                            stacks[nextStackIdx].RemoveAt(idx);
                            stacks[stackIdx].RemoveAt(idx);
                            // TODO: update score
                        }
                    }
                }
                // Check with right stack

                return true;
            }
            else
            {
                if (tc.Pos.Y + 1 == stacks[stackIdx][topCharIdx].Pos.Y)
                {
                    int floatingCharDy = stacks[stackIdx].Count;

                    // Remove from current stack
                    if (IsSameTetrisBlock(tc, stacks[stackIdx][topCharIdx]))
                    {
                        stacks[stackIdx].RemoveAt(topCharIdx);
                        remove = true;
                        // TODO: update score
                    }

                    // For left most stack, only check right and bottom
                    if (stackIdx == 0)
                    {
                        leftmostStack = true;
                    }

                    if (stackIdx == lastStackIdx)
                    {
                        rightmostStack = true;
                    }

                    // if not leftmost stack, check left stack
                    if (!leftmostStack)
                    {
                        int leftStackIdx = stackIdx - 1;
                        if (stacks[leftStackIdx].Count - 1 >= floatingCharDy)
                        {
                            if (IsSameTetrisBlock(tc, stacks[leftStackIdx][floatingCharDy]))
                            {
                                stacks[leftStackIdx].RemoveAt(floatingCharDy);
                                // TODO: update the blocks in the stack
                                remove = true;
                                // TODO: update score
                            }
                        }
                    }

                    // If not rightmost stack, check right stack
                    if (!rightmostStack)
                    {
                        int nextStackIdx = stackIdx + 1;
                        if (stacks[nextStackIdx].Count - 1 >= floatingCharDy)
                        {
                            if (IsSameTetrisBlock(tc, stacks[nextStackIdx][floatingCharDy]))
                            {
                                stacks[nextStackIdx].RemoveAt(floatingCharDy);
                                // TODO: update the blocks in the stack
                                remove = true;
                                // TODO: update score
                            }
                        }
                    }
                    // Update Teris Stack
                    if (!remove)
                        stacks[stackIdx].Add(tc);


                    return true;
                }
            }

            if (hitBottom)
                return true;

            return false;
        }

        // Use when the game finishes
        protected static bool IsTetrisBeaten(List<List<TetrisChar>> list)
        {
            foreach (List<TetrisChar> l in list)
            {
                if (l.Count != 0)
                    return false;
            }
            return true;
        }

        protected static void PrintTetrisManual()
        {
            Console.Clear();
            Console.WriteLine("Tetris - Hello World");
            Console.WriteLine("Arrows move up/down/right/left.");
            Console.WriteLine("Press 'esc' quit.");
            Console.WriteLine("Press space to pause.");
        }

        protected static void ReceiveTetrisKeyPress(TetrisChar tc, int rightMostStackIdx, ref bool live)
        {
            ConsoleKeyInfo consoleKey;
            int x, y;
            // get key and use it to set options
            consoleKey = Console.ReadKey(true);
            switch (consoleKey.Key)
            {
                case ConsoleKey.UpArrow: //UP
                                         // Do nothing    
                    break;
                case ConsoleKey.DownArrow: // DOWN

                    break;
                case ConsoleKey.LeftArrow: //LEFT
                    // if not leftmost, decrease x
                    // TODO: stop transpassing
                    x = tc.Pos.X;
                    y = tc.Pos.Y;
                    if (x != 0)
                        tc.Pos = new Point(--x, y);
                    break;
                case ConsoleKey.RightArrow: //RIGHT
                    // if not rightmost, increase x
                    // TODO: stop transpassing
                    x = tc.Pos.X;
                    y = tc.Pos.Y;
                    if (x != rightMostStackIdx)
                        tc.Pos = new Point(++x, y);
                    break;
                case ConsoleKey.Spacebar:
                    PauseTetris();
                    break;
                case ConsoleKey.Escape: //END
                    live = false;
                    break;

            }
        }


        // Mode: 6, the tetris game
        public static void PlayTetris()
        {
            bool gameLive = true;
            ConsoleKeyInfo consoleKey;

            // location info & display
            int i = 0;
            // If a character hits the stack
            bool landed = false;

            Console.CursorVisible = false;
            // TODO: check display
            PrintTetrisManual();

            // Create a stage, every character has a color, a character value, and a coordinate
            List<List<TetrisChar>> tetrisStacks = new List<List<TetrisChar>>();
            for (i = 0; i < STACKS; i++)
            {
                tetrisStacks.Add(CreateTetrisStack(MAX_STACK_HEIGHT, i));
            }

            // Draw the stacks for the 1s time
            DrawTetrisStack(tetrisStacks);

            TetrisChar droppingChar = ReturnTetrisBlock(STACKS, DROP_POINT);


            do // until escape
            {
                if (landed)
                {
                    // Drop a new character from the top
                    droppingChar = ReturnTetrisBlock(STACKS, DROP_POINT);
                    landed = false;
                }
                else
                {
                    // Flowing from top to bottom
                    for (int j = DROP_POINT; j < CONSOLE_HEIGHT; j++)
                    {
                        Console.ForegroundColor = droppingChar.ChColor;
                        // Keep moving
                        WriteAt(droppingChar.ChValue.ToString(),
                            droppingChar.Pos.X, droppingChar.Pos.Y, 500);
                        WriteAt(" ", droppingChar.Pos.X, droppingChar.Pos.Y, 0);

                        // see if a key has been pressed
                        if (Console.KeyAvailable)
                        {
                            ReceiveTetrisKeyPress(droppingChar, STACKS - 1, ref gameLive);
                            if (!gameLive)
                                return;
                        }

                        // Check for landing
                        landed = IsTetrisBlockLanded(droppingChar, tetrisStacks);

                        DrawTetrisStack(tetrisStacks);

                        if (landed)
                        {
                            break;
                        }

                        // Update dropping block
                        droppingChar.Pos = new Point(droppingChar.Pos.X, j);
                    }
                }

                if (IsTetrisBeaten(tetrisStacks))
                {
                    // Print Congrats
                    Console.WriteLine("Congratulations!");
                    return;
                }

                // Draw Tetris Stacks
                DrawTetrisStack(tetrisStacks);

            } while (gameLive);
            Console.WriteLine("Bye");
        }

        protected static void PauseTetris()
        {
            ConsoleKeyInfo consoleKey;
            for (; ; )
            {
                consoleKey = Console.ReadKey(true);

                if (consoleKey.Key == ConsoleKey.Spacebar)
                    break;
                else
                    continue;
            }

        }


    }


    public class Program
    {
        internal static int origRow;
        internal static int origCol;
        internal const int CONSOLE_WIDTH = 79;
        internal const int CONSOLE_HEIGHT = 20;

        internal const string _HW = "HELLO WORLD!";

        // https://docs.microsoft.com/en-us/dotnet/api/system.console.setcursorposition?view=net-5.0
        internal static void WriteAt(string s, int x, int y, int sleep = 0)
        {
            object lockLock = new object();
            lock (lockLock)
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
                WriteAt(hw[i].ToString(), i + 2, 2,1000);
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

                 Console.ForegroundColor = (ConsoleColor)r.Next(0, range); 
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
                    WriteAt(hw[i].ToString(), X_AXIS, j, 500);

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

        protected static void DrawRow(string s, int dx, int dy, int count)
        {
            for (int i = 0; i < count; i++)
            {
                dx += i;
                WriteAt(s, dx, dy, 0);
            }
        }

        protected static void DrawFlower(string s, int layer, int dx, int dy)
        {
            // Center
            WriteAt(s, dx, dy);
            // First layer
            for (int i = 0; i <= layer; i++)
            {
                WriteAt(s, dx, dy);
                // Top left
                dx -= 1;
                dy -= 1;
            }

        }

        protected static void PrintSqure(int dx, int dy, int len, string[,] s)
        {
            int x = dx, y = dy;
            for (int i = 0; i < len; i++)
            {
                if (i == 0 || i == len - 1) // print row
                {
                    y = dy + i;
                    x = dx;
                    for (int j = 0; j < len; j++)
                    {
                        WriteAt(s[x - dx + j, y - dy], x + j, y, 0);
                    }
                    if (i == len - 1)
                    {
                        Thread.Sleep(3000);
                        //Console.Clear();
                    }
                }
                else // print border
                {
                    x = dx;
                    y = dy + i;
                    WriteAt(s[x - dx, y - dy], x, y, 0);
                    x = dx + len - 1;
                    WriteAt(s[x - dx, y - dy], x, y, 0);
                }
            }
        }

        protected static void ShowFireFlower(int dx, int dy)
        {
            int x = dx, y = dy;
            Random r = new Random();
            int range = Enum.GetValues(typeof(ConsoleColor)).Length;

            string[,] fw = new string[,]
            {
             { "\\", "", "", "", "/" },
             { "", "\\", "", "/", "" },
             { "-", "-", ".", "-", "-" },
             { "", "/", "", "\\", "" },
             { "/", "", "", "", "\\" }
             };
            // Print center
            Console.ForegroundColor = (ConsoleColor)r.Next(0, range);
            WriteAt(fw[2, 2], x, y, 0);
            Thread.Sleep(700);

            WriteAt(" ", x, y, 0);
            // Print first layer
            Console.ForegroundColor = (ConsoleColor)r.Next(0, range);
            WriteAt(fw[2, 1], dx - 1, dy, 0);
            WriteAt(fw[2, 3], dx + 1, dy, 0);

            int jump = 2;
            // Reset coordinates
            x = dx - 1;
            y = dy - 1;
            WriteAt(fw[1, 1], x, y, 0);
            WriteAt(fw[1, 3], x + jump, y, 0);
            WriteAt(fw[3, 1], x, y + jump, 0);
            WriteAt(fw[3, 3], x + jump, y + jump, 0);
            Thread.Sleep(700);

            WriteAt(" ", dx - 1, dy, 0);
            WriteAt(" ", dx + 1, dy, 0);
            WriteAt(" ", x, y, 0);
            WriteAt(" ", x + jump, y, 0);
            WriteAt(" ", x, y + jump, 0);
            WriteAt(" ", x + jump, y + jump, 0);

            // Print second layer
            Console.ForegroundColor = (ConsoleColor)r.Next(0, range);

            WriteAt(fw[2, 0], dx - 2, dy, 0);
            WriteAt(fw[2, 4], dx + 2, dy, 0);

            jump += 2;

            // Reset coordinates
            x = dx - 2;
            y = dy - 2;

            WriteAt(fw[0, 0], x, y, 0);
            WriteAt(fw[0, 4], x + jump, y, 0);
            WriteAt(fw[4, 0], x, y + jump, 0);
            WriteAt(fw[4, 4], x + jump, y + jump, 0);

            Thread.Sleep(700);

            WriteAt(" ", dx - 2, dy, 0);
            WriteAt(" ", dx + 2, dy, 0);

            WriteAt(" ", x, y, 0);
            WriteAt(" ", x + jump, y, 0);
            WriteAt(" ", x, y + jump, 0);
            WriteAt(" ", x + jump, y + jump, 0);

        }

        protected static void PrintFireworks(string[,] fw, int layer)
        {
            Console.CursorVisible = false;
            int dx = 10, dy = 10;
            int dimsion = 2;
            for (int i = 0; i < layer; i++)
            {
                PrintSqure(dx, dy, dimsion, fw);
                dimsion += 2;
                dx -= 1;
                dy -= 1;
            }

        }

        protected static void PrintFireworksBox(List<Tetris.TetrisChar> list)
        {
            foreach (Tetris.TetrisChar tc in list)
            {
                Console.ForegroundColor = tc.ChColor;
                WriteAt(tc.ChValue.ToString(), tc.Pos.X, tc.Pos.Y, 0);
            }

        }

        // TODO: Detect console borders before drawing
        protected static List<Tetris.TetrisChar> CreateFireworksBox(int x, int y)
        {
            Random r = new Random(10);
            const int dimension = 5;
            int range = Enum.GetValues(typeof(ConsoleColor)).Length;
            int dx = x, dy = y;
            List<Tetris.TetrisChar> list = new List<Tetris.TetrisChar>();

            int i = 0, j = 0;

            for (i = 0; i < dimension + 5; i++)
            {
                for (j = 0; j < dimension; j++)
                {
                    int idx = r.Next(0, _HW.Length);
                    Console.ForegroundColor = (ConsoleColor)r.Next(0, range);
                    if (_HW[idx] == ' ' || _HW[idx] == '!')
                        idx--;

                    list.Add(new Tetris.TetrisChar(_HW[idx], Console.ForegroundColor,
                        new Point(dx + i, dy + j)));
                }

            }

            return list;
        }


        // MODE = 5
        protected static void StartFireworks(string hw)
        {
            const int TOP = 20;
            const int BOTTOM = 40;
            const int X_AXIS = 10;
            Console.CursorVisible = false;
            int i, j;

            List<Tetris.TetrisChar> list = CreateFireworksBox(X_AXIS - 3, 26);

            PrintFireworksBox(list);

            for (i = 0; i < hw.Length; i++)
            {
                // Flowing from bottom to top
                for (j = BOTTOM - 15; j > TOP - i; j--)
                {
                    WriteAt(hw[i].ToString(), X_AXIS, j, 100);
                    WriteAt( x:X_AXIS, y:j, s:hw[i].ToString());
                    if (j != i - TOP + 1)
                        WriteAt(" ", X_AXIS, j, 1);
                }

                ShowFireFlower(X_AXIS, j);
                PrintFireworksBox(list);
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
        protected static void PrintHelloWordOutline(string dot, string hw)
        {
            Random r = new Random();
            int range = Enum.GetValues(typeof(ConsoleColor)).Length;

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
                Console.ForegroundColor = (ConsoleColor)r.Next(0, range);
                for (int i = 0; i < bitImage[c].GetLength(0); i++)

                    for (int j = 0; j < bitImage[c].GetLength(1); j++)
                    {
                        if (bitImage[c][i, j] == 1)
                            WriteAt(dot, j + dx, i);
                    }
                dx += bitImage[c].GetLength(0);
            }
        }

        protected static void SayHelloWorld()
        {
            List<string> hw = new List<string>()
            { "Hello World", "こんにちは世界","你好 世界",
                "नमस्ते दुनिया","Hola Mundo","مرحبا بالعالم",
            "Hallo Wereld","Bonjour le monde","Hallo Welt",
                "Aloha honua","Ciao mondo","안녕하세요 세계",
                "ഹലോ വേൾഡ്","Olá Mundo", "Привет мир"};
            Random r = new Random();
            int range = Enum.GetValues(typeof(ConsoleColor)).Length;
            Console.CursorVisible = false;

            // Use bold font
            Console.Write("\x1b[1m");
            // Change font size
            for (; ; )
            {
                Console.ForegroundColor = (ConsoleColor)r.Next(0, range);

                int i = r.Next(0, hw.Count);
                Console.WriteLine(hw[i]);
                Thread.Sleep(1000);
                Console.Clear();
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
                default:
                    Console.ForegroundColor = ConsoleColor.Black;
                    break;
            }
        }

        static char GetRandomAsciiChar()
        {
            int seed = (int)DateTime.Now.Ticks;
            Random random = new Random(seed);

            // Generate a random integer within the ASCII range of English alphabet
            int randomNumber = random.Next(0x0041, 0x007B);

            // Convert the integer to a Unicode character
            return (char)randomNumber;
        }

        static char GetRandomUnicodeJpChar()
        {
            int seed = (int)DateTime.Now.Ticks;
            // Create a Random object
            Random random = new Random(seed);

            // Generate a random integer within the range of valid Unicode characters
            int randomNumber = random.Next(0x30A0, 0x30FF);

            // Convert the integer to a Unicode character
            return (char)randomNumber;

        }

        static char GetCharForMatrix()
        {
            Random randomChance = new Random();
            // Generate a random number between 0 and 100
            int randomNumber = randomChance.Next(0, 101);

            // Check if the random number is less than or equal to 10 (10% chance)
            if (randomNumber <= 25)
            {
                // Call your function here
                return GetRandomUnicodeJpChar();
            }
            else
            {
                return GetRandomAsciiChar();
            }
        }

        static char GetRandomUnicodeChar()
        {
            Random random = new Random();

            // Generate a random integer within the range of valid Unicode characters
            int randomNumber = random.Next(0x0000, 0x10FFFF);

            // Convert the integer to a Unicode character
            return (char)randomNumber;
        }

        // The Matrix Effect
        protected static void EnterTheMatrix()
        {
            List<KeyValuePair<char, int>> ms = new List<KeyValuePair<char, int>>();
            // Generate a random char and add to the top of the list
            int i;
            Random r = new Random();
            const int SIZE = 12;
            int dx = r.Next(0, CONSOLE_WIDTH);
            // head y coordinate
            int dy = 0;
            // y coordinate except for the head
            int y = 0;
            // Rest string
            bool rest = false;

            // Hide cursor
            Console.CursorVisible = false;

            for (; ; )
            {
                // Clear last character
                WriteAt(" ", dx, y, 0);
                //if(!hwMode)
                // Check # of characters in the list

                if (ms.Count < SIZE)
                {
                    // Add a random character at the head of the list

                    char c = GetCharForMatrix();
                    ms.Insert(0, new KeyValuePair<char, int>(c, SIZE));
                }

                // Update dy in each iteration
                dy++;
                //
                // Print characters and color
                for (i = 0; i < ms.Count; i++)
                {
                    ChangeMatrixColor(i);

                    // Update y coordinate
                    y = dy - i;
                    WriteAt(ms[i].Key.ToString(), dx, y, 0);

                    // Decrease counter
                    if (ms[i].Value == 0)
                    {
                        // Remove from the list
                        ms.RemoveAt(i);
                    }
                    else
                    {
                        ms[i] = new KeyValuePair<char, int>(ms[i].Key, ms[i].Value - 1);
                    }


                    // y first touches the bottom of the console
                    if (y == CONSOLE_HEIGHT)
                    {
                        rest = true;
                    }
                    else
                        rest = false;

                    // When y touches the bottom, then re-assign dx and dy
                    // TODO: double check i==ms.Count-1
                    if (i == ms.Count - 1 && rest)
                    {
                        dx = r.Next(0, CONSOLE_WIDTH);
                        dy = 0;
                        ms.Clear();
                        rest = false;
                    }
                }
                Thread.Sleep(200);
            }
        }

        protected static void EnterTheMatrixHw()
        {
            string hw = "Hello World!";
            List<KeyValuePair<char, int>> ms = new List<KeyValuePair<char, int>>();
            // Generate a random char and add to the top of the list
            int i;
            Random r = new Random();
            int SIZE = hw.Length;
            int dx = r.Next(0, CONSOLE_WIDTH);
            // head y coordinate
            int dy = 0;
            // y coordinate except for the head
            int y = 0;
            // Rest string
            bool rest = false;

            // Hide cursor
            Console.CursorVisible = false;

            // hw index
            int ind = 0;
            for (; ; )
            {
                // Clear last character
                WriteAt(" ", dx, y, 0);
                //if(!hwMode)
                // Check # of characters in the list

                if (ms.Count < SIZE)
                {
                    // Add a character at the head of the list
                    ms.Insert(0, new KeyValuePair<char, int>(hw[ind], SIZE));
                    ind++;
                }
                else
                {
                    ind %= SIZE;
                    ms.Insert(0, new KeyValuePair<char, int>(hw[ind], SIZE));
                    ind++;
                }

                // Update dy in each iteration
                dy++;
                //
                // Print characters and color
                for (i = 0; i < ms.Count; i++)
                {
                    ChangeMatrixColor(i);

                    // Update y coordinate
                    y = dy - i;
                    WriteAt(ms[i].Key.ToString(), dx, y, 0);

                    // Decrease counter
                    if (ms[i].Value == 0)
                    {
                        // Remove from the list
                        ms.RemoveAt(i);
                    }
                    else
                    {
                        ms[i] = new KeyValuePair<char, int>(ms[i].Key, ms[i].Value - 1);
                    }

                    // y first touches the bottom of the console
                    if (y == CONSOLE_HEIGHT)
                    {
                        rest = true;
                    }
                    else
                        rest = false;

                    // When y touches the bottom, then re-assign dx and dy
                    if (i == ms.Count - 2 && rest)
                    {
                        dx = r.Next(0, CONSOLE_WIDTH);
                        dy = 0;
                        ms.Clear();
                        rest = false;
                        ind = 0;
                    }
                }
                Thread.Sleep(200);
            }
        }


        // The Matrix Effect
        protected static void TheMatrixHw()
        {
            string hw = "Hello World!";
            List<KeyValuePair<char, int>> ms = new List<KeyValuePair<char, int>>();
            // Generate a random char and add to the top of the list
            int i;
            Random r = new Random();

            int dx = r.Next(0, CONSOLE_WIDTH);
            // head y coordinate
            int dy = hw.Length;
            // y coordinate except for the head
            int y = 0;
            // Rest string
            bool rest = false;

            // Hide cursor
            Console.CursorVisible = false;

            // Initilize list
            foreach (char c in hw)
            {
                ms.Add(new KeyValuePair<char, int>(c, hw.Length));
            }

            for (; ; )
            {
                // Clear last character
                WriteAt(" ", dx, y, 0);

                // Update dy in each iteration
                dy++;
                //
                // Print characters and color
                for (i = 0; i < ms.Count; i++)
                {
                    ChangeMatrixColor(i);

                    // Update y coordinate
                    y = dy - i;
                    WriteAt(ms[i].Key.ToString(), dx, y, 0);

                    // Decrease counter
                    if (ms[i].Value == 0)
                    {
                        // Reset counter
                        ms[i] = new KeyValuePair<char, int>(ms[i].Key, hw.Length);
                    }
                    else
                    {
                        ms[i] = new KeyValuePair<char, int>(ms[i].Key, ms[i].Value - 1);
                    }

                    // y first touches the bottom of the console
                    if (y == CONSOLE_HEIGHT)
                    {
                        rest = true;
                    }

                    // When y touches the bottom, then re-assign dx and dy
                    if (i == ms.Count - 1 && rest)
                    {
                        dx = r.Next(0, CONSOLE_WIDTH);
                        dy = hw.Length;
                        // TODO: fix this
                        for (int j = CONSOLE_HEIGHT; j > 0; j--)
                        {
                            WriteAt(" ", dx, j, 0);
                        }
                        rest = false;
                    }
                }
                Thread.Sleep(200);
            }
        }
        // MAIN
        public static void Main()
        {
            // TODO: Change mode before each run
            const int MODE = 8;

            switch (MODE)
            {
                case 0: // print hello world;
                    Console.WriteLine(_HW);
                    Console.Read();
                    break;
                case 1: // print from left to right
                    PrintLeftToRight(_HW);
                    Console.Read();
                    break;
                case 2: // print from right to left
                    PrintRightToLeft(_HW);
                    Console.Read();
                    break;
                case 3: // print from top to bottom
                    char[] reversedString = _HW.ToCharArray();
                    Array.Reverse(reversedString);
                    PrintTopToBottom(new string(reversedString));
                    Console.Read();
                    break;
                case 4: // print from bottom to top
                    PrintBottomToTop(_HW);
                    Console.Read();
                    break;
                case 5: // Firework effect, characters going from bottom to top
                    StartFireworks(_HW);
                    Console.Read();
                    break;
                case 6: // Tetris game
                    Tetris.PlayTetris();
                    break;
                case 7: // Hello world outline with characters
                    PrintHelloWordOutline("/", _HW);
                    Console.Read();
                    break;
                case 8: // Greetings in random language
                    SayHelloWorld();
                    Console.Read();
                    break;
                case 9: // The Matrix, digital rain
                    for (int i = 0; i < 30; i++)
                    {
                        ThreadStart child = new ThreadStart(EnterTheMatrix);
                        Thread childThread = new Thread(child);
                        childThread.Start();
                        Thread.Sleep(1000);
                    }
                    break;
                case 10: // The Matrix HelloWorld!
                    for (int i = 0; i < 15; i++)
                    {
                        ThreadStart child = new ThreadStart(EnterTheMatrixHw);
                        Thread childThread = new Thread(child);
                        childThread.Start();
                        Thread.Sleep(2000);
                    }
                    break;
                
                case 11: // Snake game-1, control "Hello World"
                    Snake(_HW);
                    break;
                case 12: // Snake game-2, append characters
                    Snake2(_HW);
                    break;
                case 13: // Snake game-3, a moving snake
                    break;
                case 14: // pac-man-1, hello world is the pac-man
                    break;
                case 15: // pac-man-2, use hello world to build the map
                    break;
                case 16: // Space Invader?
                    break;
                default:
                    Console.WriteLine("The mode is yet to be implemented");
                    break;
            }
        }
    }

}
