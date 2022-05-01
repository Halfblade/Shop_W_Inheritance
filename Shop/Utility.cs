using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;


namespace FSPG
{
    public static class Utility
    {
        [DllImport("user32.dll")]
        static extern bool LockWindowUpdate(IntPtr hWndLock);
        [DllImport("user32.dll")]
        static extern ushort GetAsyncKeyState(int vKey);
        [DllImport("user32.dll")]
        static extern int GetSystemMetrics(int nIndex);        
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, uint wFlags);
        [DllImport("user32.dll")]
        static extern bool GetWindowRect(HandleRef hWnd, out RECT lpRect);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);
        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [StructLayout(LayoutKind.Sequential)]
        struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        // Win32 constants
        const int STD_INPUT_HANDLE = -10;
        const int STD_OUTPUT_HANDLE = -11;
        const int STD_ERROR_HANDLE = -12;
        const int SM_CXSCREEN = 0;
        const int SM_CYSCREEN = 1;
        const int SWP_NOSIZE = 1;
        const uint ENABLE_PROCESSED_INPUT = 0x0001;
        const uint ENABLE_LINE_INPUT = 0x0002;
        const uint ENABLE_ECHO_INPUT = 0x0004;
        const uint ENABLE_WINDOW_INPUT = 0x0008;
        const uint ENABLE_MOUSE_INPUT = 0x0010;
        const uint ENABLE_INSERT_MODE = 0x0020;
        const uint ENABLE_QUICK_EDIT_MODE = 0x0040;
        const uint ENABLE_EXTENDED_FLAGS = 0x0080;
        const uint ENABLE_AUTO_POSITION = 0x0100;
        const uint ENABLE_PROCESSED_OUTPUT = 0x0001;
        const uint ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002;
        
        // class fields
        static Random mPRNG = new Random();
        static bool mGoodRead = true;

        public static void SetupWindow(string title, int width, int height, bool center = true)
        {
            Console.Title = title;

            Console.SetWindowSize(width, height);
            Console.SetBufferSize(width, height);

            if (center)
            {
                RECT rect;
                if (!GetWindowRect(new HandleRef(
                    Process.GetCurrentProcess().GetLifetimeService(),
                    Process.GetCurrentProcess().MainWindowHandle), out rect))
                    return;

                int sw = GetSystemMetrics(SM_CXSCREEN);
                int sh = GetSystemMetrics(SM_CYSCREEN);
                int ww = rect.Right - rect.Left;
                int wh = rect.Bottom - rect.Top;
                int x = sw / 2 - ww / 2;
                int y = sh / 2 - wh / 2;
                SetWindowPos(Process.GetCurrentProcess().MainWindowHandle,
                    0, x, y, 0, 0, SWP_NOSIZE);
            }
        }

        // Locks/unlocks the memory used by Console to display output.
        // Helps reduce screen flicker when:
        // lock applied before Clear(), and unlock applied after Write(s)
        public static void LockConsole(bool applyLock)
        {
            if (applyLock)
            {
                LockWindowUpdate(Process.GetCurrentProcess().MainWindowHandle);
            }
            else
            {
                LockWindowUpdate(IntPtr.Zero);
            }
        }

        public static void EOLWrap(bool on)
        {
            uint mode;
            IntPtr stdoutHandle = GetStdHandle(STD_OUTPUT_HANDLE);

            if (!GetConsoleMode(stdoutHandle, out mode))
                return;

            if (on)
                mode |= ENABLE_WRAP_AT_EOL_OUTPUT;
            else
                mode &= ~ENABLE_WRAP_AT_EOL_OUTPUT;

            SetConsoleMode(stdoutHandle, mode);
        }

        public static void WriteCentered(string msg, int rowOffset = 0)
        {
            int colOffset = (Console.WindowWidth / 2) - (msg.Length / 2);
            rowOffset = (Console.WindowHeight / 2) + rowOffset;

            if (colOffset < 0)
                colOffset = 0;
            else if (colOffset >= Console.WindowWidth)
                colOffset = Console.WindowWidth - 1;

            if (rowOffset < 0)
                rowOffset = 0;
            else if (rowOffset >= Console.WindowHeight)
                rowOffset = Console.WindowHeight - 1;

            Console.SetCursorPosition(colOffset, rowOffset);
            Console.Write(msg); 
        }

        public static bool GetKeyState(ConsoleKey key)
        {
            return (GetAsyncKeyState((int)key) != 0);
        }

        public static bool GetKeyOnce(ConsoleKey key)
        {
            return (((GetAsyncKeyState((int)key) & 0x01)) > 0);
        }

        public static void WaitForEnterKey(bool msg = true)
        {
            if (msg)
            {
                WriteCentered("Press Enter to Continue", Console.WindowHeight / 2);
            }

            //while (!GetKeyState(ConsoleKey.Enter))
            //{
            //    Thread.Sleep(1);
            //}

            ConsoleKeyInfo key = new ConsoleKeyInfo('Z', ConsoleKey.Z, false, false, false);

            while (key.Key != ConsoleKey.Enter)
            {
                while (!Console.KeyAvailable)
                {
                    Thread.Sleep(1);
                }

                key = Console.ReadKey();
            }

            // TODO: Flush keys to get rid of extra input for GetAsyncKeyState.
        }

       
        public static void DrawBox(int left, int top, int width, int height, bool dbl)
        {
            //          wchar_t const * SingleLine = L"┌─┐│└┘",
            //*DoubleLine = L"╔═╗║╚╝",
            //*Set = dbl ? DoubleLine : SingleLine;

            //          Show(left, top, Set[0]);
            //          for (int col = 0; col < width - 2; col++)
            //              Show(Set[1]);
            //          Show(Set[2]);

            //          int x = left + width - 1, y = top + 1;
            //          for (int row = 0; row < height - 2; row++, y++)
            //          {
            //              Show(left, y, Set[3]);
            //              Show(x, y, Set[3]);
            //          }

            //          y = top + height - 1;
            //          Show(left, y, Set[4]);
            //          for (int col = 0; col < width - 2; col++)
            //              Show(Set[1]);
            //          Show(Set[5]);

            string singleLine = "┌─┐│└┘";
            string doubleLine = "╔═╗║╚╝";
            string set = dbl ? doubleLine : singleLine;

            Console.SetCursorPosition(left, top);
            Console.Write(set[0]);

            for (int col = 0; col < width - 2; col++)
            {
                Console.Write(set[1]);
            }

            Console.Write(set[2]);

            int x = left + width - 1;
            int y = top + 1;

            for (int row = 0; row < height - 2; row++, y++)
            {
                Console.SetCursorPosition(left, y);
                Console.Write(set[3]);

                Console.SetCursorPosition(x, y);
                Console.Write(set[3]);
            }

            y = top + height - 1;

            Console.SetCursorPosition(left, y);
            Console.Write(set[4]);

            for (int col = 0; col < width - 2; col++)
            {
                Console.Write(set[1]);
            }

            Console.Write(set[5]);
        }

        // A Unix timestamp.
        // Returns the number of seconds since The Epoch (January 1 1970).
        public static int UnixNow()
        {
            return (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        // Seeds the pseudo-random number generator.
        public static void SeedPRNG(int seed)
        {
            mPRNG = new Random(seed);
        }

        // Returns the next random number from the pseudo-random number generator.
        public static int Rand()
        {
            return mPRNG.Next();
        }

        // The success of the last Read call.
        // False if last Read failed, True otherwise.
        public static bool IsReadGood()
        {
            return mGoodRead;
        }

        public static byte ReadByte()
        {
            string raw = "";
            byte result = 0;

            try
            {
                raw = Console.ReadLine();
                result = Convert.ToByte(raw);
                mGoodRead = true;
            }
            catch (Exception)
            {
                mGoodRead = false;
            }

            return result;
        }

        public static char ReadChar()
        {
            string raw = "";
            char result = '\0';

            raw = Console.ReadLine();

            if (raw.Length < 1)
            {
                mGoodRead = false;
                
            }
            else
            {
                result = raw[0];
                mGoodRead = true;
            }

            return result;
        }
        
        public static sbyte ReadSByte()
        {
            string raw = "";
            sbyte result = 0;

            try
            {
                raw = Console.ReadLine();
                result = Convert.ToSByte(raw);
                mGoodRead = true;
            }
            catch (Exception)
            {
                mGoodRead = false;
            }

            return result;
        }

        public static short ReadShort()
        {
            string raw = "";
            short result = 0;

            try
            {
                raw = Console.ReadLine();
                result = Convert.ToInt16(raw);
                mGoodRead = true;
            }
            catch (Exception)
            {
                mGoodRead = false;
            }

            return result;
        }

        public static ushort ReadUShort()
        {
            string raw = "";
            ushort result = 0;

            try
            {
                raw = Console.ReadLine();
                result = Convert.ToUInt16(raw);
                mGoodRead = true;
            }
            catch (Exception)
            {
                mGoodRead = false;
            }

            return result;
        }

        public static int ReadInt()
        {
            string raw = "";
            int result = 0;

            try
            {
                raw = Console.ReadLine();
                result = Convert.ToInt32(raw);
                mGoodRead = true;
            }
            catch (Exception)
            {
                mGoodRead = false;
            }

            return result;
        }

        public static uint ReadUInt()
        {
            string raw = "";
            uint result = 0;

            try
            {
                raw = Console.ReadLine();
                result = Convert.ToUInt32(raw);
                mGoodRead = true;
            }
            catch (Exception)
            {
                mGoodRead = false;
            }

            return result;
        }

        public static long ReadLong()
        {
            string raw = "";
            long result = 0;

            try
            {
                raw = Console.ReadLine();
                result = Convert.ToInt64(raw);
                mGoodRead = true;
            }
            catch (Exception)
            {
                mGoodRead = false;
            }

            return result;
        }

        public static ulong ReadULong()
        {
            string raw = "";
            ulong result = 0;

            try
            {
                raw = Console.ReadLine();
                result = Convert.ToUInt64(raw);
                mGoodRead = true;
            }
            catch (Exception)
            {
                mGoodRead = false;
            }

            return result;
        }

        public static float ReadFloat()
        {
            string raw = "";
            float result = 0.0f;

            try
            {
                raw = Console.ReadLine();
                result = Convert.ToSingle(raw);
                mGoodRead = true;
            }
            catch (Exception)
            {
                mGoodRead = false;
            }

            return result;
        }

        public static double ReadDouble()
        {
            string raw = "";
            double result = 0.0;

            try
            {
                raw = Console.ReadLine();
                result = Convert.ToDouble(raw);
                mGoodRead = true;
            }
            catch (Exception)
            {
                mGoodRead = false;
            }

            return result;
        }
    }
}
