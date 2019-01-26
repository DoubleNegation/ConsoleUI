using System;

namespace ConsoleUI {

    class ConsoleUtils {

        private char[] consoleChars;
        private int lastWidth, lastHeight;

        public ConsoleUtils() {
            consoleChars = new char[Console.WindowWidth * Console.WindowHeight];
            lastWidth = Console.WindowWidth;
            lastHeight = Console.WindowHeight;
        }

        public void Update(Window w) {
            if(lastHeight != Console.WindowHeight || lastWidth != Console.WindowWidth) {
                lastWidth = Console.WindowWidth;
                lastHeight = Console.WindowHeight;
                try {
                    Console.Clear();
                } catch(System.IO.IOException) {
                    // happens when the window height becomes 0
                }
                w.ConsoleWindowInvalidated();
            }
        }

        public char this[int x, int y] {
            get {
                return consoleChars[y * Console.WindowWidth + x];
            }
            set {
                consoleChars[y * Console.WindowWidth + x] = value;
                Console.SetCursorPosition(x, y);
                Console.Write(value);
            }
        }

        public static T[] CopyAndFlipArray<T>(T[] arr) {
            T[] copy = new T[arr.Length];
            for(int i = 0; i < arr.Length; i++) {
                copy[arr.Length - 1 - i] = arr[i];
            }
            return copy;
        }
    }

    internal class ConsoleBuffer {

        private int width, height;
        private char[] chars;
        private Color[] foreground;
        private Color[] background;
        private Color activeBackground;
        private Color activeForeground;

        public ConsoleBuffer(int width, int height, Color initialBg, Color defaultFg) {
            this.width = width;
            this.height = height;
            chars = new char[width * height];
            foreground = new Color[width * height];
            background = new Color[width * height];
            for(int i = 0; i < width * height; i++) {
                chars[i] = ' ';
                foreground[i] = defaultFg;
                background[i] = initialBg;
            }
        }

        public char this[int x, int y] {
            get {
                return chars[y * width + x];
            } set {
                chars[y * width + x] = value;
                foreground[y * width + x] = activeForeground;
                background[y * width + x] = activeBackground;
            }
        }

        public Color Background {
            get {
                return activeBackground;
            } set {
                activeBackground = value;
            }
        }

        public Color Foreground {
            get {
                return activeForeground;
            } set {
                activeForeground = value;
            }
        }

        public Color GetBackgroundAt(int x, int y) {
            return background[y * width + x];
        }

        public void SetBackgroundAt(int x, int y, Color c) {
            background[y * width + x] = c;
        }

        public Color GetForegroundAt(int x, int y) {
            return foreground[y * width + x];
        }

        public void SetForegroundAt(int x, int y, Color c) {
            foreground[y * width + x] = c;
        }

        public int Width {
            get {
                return width;
            }
        }

        public int Height {
            get {
                return height;
            }
        }

        public void DrawToConsole(int x, int y) {
            for(int py = y; py < y + height; py++) {
                Console.SetCursorPosition(x, py);
                for(int px = x; px < x + width; px++) {
                    Console.ForegroundColor = foreground[(py - y) * width + (px - x)].ConsoleColorValue;
                    Console.BackgroundColor = background[(py - y) * width + (px - x)].ConsoleColorValue;
                    Console.Write(chars[(py - y) * width + (px - x)]);
                }
            }
        }

        public void copyTo(ConsoleBuffer buffer) {
            if(buffer.Width != width || buffer.Height != height) throw new ArgumentException("Sizes of buffers don't match.");
            for(int i = 0; i < width * height; i++) {
                buffer.Foreground = foreground[i];
                buffer.Background = background[i];
                buffer[i % width, i / width] = chars[i];
            }
        }

    }

}

