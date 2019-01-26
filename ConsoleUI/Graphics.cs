using System;
using System.Collections.Generic;

namespace ConsoleUI {

    public class Graphics {

        private ConsoleBuffer canvas;
        private int x, y, width, height;
        private int translatedX, translatedY;
        private int limitedWidth, limitedHeight;
        private Color color = Color.BLACK;

        public Color Color {
            get {
                return color;
            }
            set {
                color = value == null ? Color.BLACK : value;
            }
        }

        internal Graphics(ConsoleBuffer canvas, int x, int y, int width, int height) {
            this.canvas = canvas;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            translatedX = 0;
            translatedY = 0;
            limitedWidth = width - x;
            limitedHeight = height - y;
        }

        public void Translate(int x, int y, int width, int height) {
            if (x == 0 && y == 0 && width == 0 && height == 0) return;
            if (x >= 0 && y >= 0 && width <= 0 && height <= 0) {
                if (limitedWidth - x + width <= 0) {
                    throw new ArgumentException("Remaining space on the x axis is <= 0.");
                } else if (limitedHeight - y + height <= 0) {
                    throw new ArgumentException("Remaining space on the y axis is <= 0.");
                }
                translatedX += x;
                translatedY += y;
                limitedWidth += width;
                limitedHeight += height;
            } else if (x <= 0 && y <= 0 && width >= 0 && height >= 0) {
                if (translatedX - x < 0 || limitedWidth - width > this.width - this.x) {
                    throw new ArgumentException("Outside of bounds on the x axis.");
                } else if (translatedY - y < 0 || limitedHeight - height > this.height - this.y) {
                    throw new ArgumentException("Outside of bounds on the y axis.");
                }
                translatedX += x;
                translatedY += y;
                limitedWidth += width;
                limitedHeight += height;
            } else {
                throw new ArgumentException("Invalid combination of arguments to resize the translation area.");
            }
        }

        private int GetActualX(int providedX) {
            if (providedX > limitedWidth) throw new ArgumentException("X coordinate out of range.");
            return x + translatedX + providedX;
        }
        private int GetActualY(int providedY) {
            if (providedY > limitedHeight) throw new ArgumentException("Y coordinate out of range.");
            return y + translatedY + providedY;
        }

        private void ValidateWidth(int providedX, int providedWidth) {
            if (providedX + providedWidth > limitedWidth) throw new ArgumentException("Width out of range.");
        }

        private void ValidateHeight(int providedY, int providedHeight) {
            if (providedY + providedHeight > limitedHeight) throw new ArgumentException("Height out of range. (" + providedY + " " + providedHeight + " " + limitedHeight + ")");
        }

        public void FillRect(int x, int y, int width, int height) {
            ValidateWidth(x, width);
            ValidateHeight(y, height);
            x = GetActualX(x);
            y = GetActualY(y);
            canvas.Background = color;
            for (int py = 0; py < height; py++) {
                for (int px = 0; px < width; px++) {
                    canvas[x + px, y + py] = ' ';
                }
            }
        }

        public void FillRect(char c, int x, int y, int width, int height) {
            ValidateWidth(x, width);
            ValidateHeight(y, height);
            x = GetActualX(x);
            y = GetActualY(y);
            canvas.Foreground = color;
            for (int py = 0; py < height; py++) {
                for (int px = 0; px < width; px++) {
                    canvas.Background = canvas.GetBackgroundAt(x + px, y + py);
                    canvas[x + px, y + py] = c;
                }
            }
        }

        public void FillBackgroundRect(int x, int y, int width, int height) {
            ValidateWidth(x, width);
            ValidateHeight(y, height);
            x = GetActualX(x);
            y = GetActualY(y);
            for (int py = 0; py < height; py++) {
                for (int px = 0; px < width; px++) {
                    canvas.SetBackgroundAt(x + px, y + py, color);
                }
            }
        }

        public void FillForegroundRect(int x, int y, int width, int height) {
            ValidateWidth(x, width);
            ValidateHeight(y, height);
            x = GetActualX(x);
            y = GetActualY(y);
            for (int py = 0; py < height; py++) {
                for (int px = 0; px < width; px++) {
                    canvas.SetForegroundAt(x + px, y + py, color);
                }
            }
        }

        public void DrawText(int x, int y, String text) {
            ValidateWidth(x, text.Length);
            ValidateHeight(y, 1);
            x = GetActualX(x);
            y = GetActualY(y);
            canvas.Foreground = color;
            foreach (char c in text) {
                canvas.Background = canvas.GetBackgroundAt(x, y);
                canvas[x, y] = c;
                x++;
            }
        }

        public void DrawLine(int x1, int y1, int x2, int y2) {
            ValidateWidth(Math.Min(x1, x2), Math.Abs(x1 - x2));
            ValidateHeight(Math.Min(y1, y2), Math.Abs(y1 - y2));
            x1 = GetActualX(x1);
            x2 = GetActualX(x2);
            y1 = GetActualY(y1);
            y2 = GetActualY(y2);
            canvas.Foreground = color;
            int xDiff = Math.Abs(x1 - x2);
            int yDiff = Math.Abs(y1 - y2);
            int xMin = Math.Min(x1, x2);
            int yMin = Math.Min(y1, y2);
            if(xDiff == 0 && yDiff == 0) return /*or we would divide by 0*/;
            if(xDiff >= yDiff) {
                decimal m = ((decimal)(y1 - y2)) / ((decimal)(x1 - x2));
                decimal b = -m * x1 + y1;
                int length = xDiff;
                for(int i = 0; i <= length; i++) {
                    int x = xMin + i;
                    int y = (int)(m * x + b);
                    canvas[x, y] = '█';
                }
            } else {
                decimal m = ((decimal)(x1 - x2)) / ((decimal)(y1 - y2));
                decimal b = -m * y1 + x1;
                int length = yDiff;
                for(int i = 0; i <= length; i++) {
                    int y = yMin + i;
                    int x = (int)(m * y + b);
                    canvas[x, y] = '█';
                }
            }
        }

    }

    public class Color {
        public static readonly Color BLACK = new Color(ConsoleColor.Black);
        public static readonly Color BLUE = new Color(ConsoleColor.DarkBlue);
        public static readonly Color GREEN = new Color(ConsoleColor.DarkGreen);
        public static readonly Color DARK_CYAN = new Color(ConsoleColor.DarkCyan);
        public static readonly Color RED = new Color(ConsoleColor.DarkRed);
        public static readonly Color PURPLE = new Color(ConsoleColor.DarkMagenta);
        public static readonly Color BROWN = new Color(ConsoleColor.DarkYellow);
        public static readonly Color LIGHT_GRAY = new Color(ConsoleColor.Gray);
        public static readonly Color GRAY = new Color(ConsoleColor.DarkGray);
        public static readonly Color LIGHT_BLUE = new Color(ConsoleColor.Blue);
        public static readonly Color LIME = new Color(ConsoleColor.Green);
        public static readonly Color CYAN = new Color(ConsoleColor.Cyan);
        public static readonly Color PINK = new Color(ConsoleColor.Red);
        public static readonly Color MAGENTA = new Color(ConsoleColor.Magenta);
        public static readonly Color YELLOW = new Color(ConsoleColor.Yellow);
        public static readonly Color WHITE = new Color(ConsoleColor.White);
        public ConsoleColor ConsoleColorValue {
            get {
                return colorCode;
            }
        }
        private ConsoleColor colorCode;
        private Color(ConsoleColor colorCode) {
            this.colorCode = colorCode;
        }
    }

}

