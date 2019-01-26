using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ConsoleUI {

    class Label : Component {
        
        private String text;
        private DirectionalConstants horiz = DirectionalConstants.LEFT;
        private DirectionalConstants vert = DirectionalConstants.CENTER;

        public Label(String text) {
            Text = text;
        }

        public String Text {
            get {
                return text;
            } set {
                text = value;
                Invalidate();
            }
        }

        public DirectionalConstants HorizontalAlignment {
            get {
                return horiz;
            } set {
                if(value != DirectionalConstants.LEFT
                        && value != DirectionalConstants.CENTER
                        && value != DirectionalConstants.RIGHT) {
                    throw new ArgumentException("Invalid Horizontal Alignment value: " + horiz);
                } else {
                    horiz = value;
                    Invalidate();
                }
            }
        }

        public DirectionalConstants VerticalAlignment {
            get {
                return vert;
            } set {
                if(value != DirectionalConstants.TOP
                        && value != DirectionalConstants.CENTER
                        && value != DirectionalConstants.BOTTOM) {
                    throw new ArgumentException("Invalid Vertical Alignment value: " + vert);
                } else {
                    vert = value;
                    Invalidate();
                }
            }
        }

        public override void PaintComponent(Graphics g) {
            Color fg = Foreground;
            Color bg = Background;
            fg = fg == null ? Color.LIGHT_BLUE : fg;
            bg = bg == null ? Color.BLACK : bg;
            Size size = GetSize();
            g.Color = bg;
            g.FillBackgroundRect(0, 0, size.Width, size.Height);
            g.Color = fg;
            List<String> lines = text.Split("\n").OfType<String>().ToList();
            int lineNum = 0;
            if(vert == DirectionalConstants.TOP) {
                lineNum = 0;
            } else if(vert == DirectionalConstants.CENTER) {
                int i = size.Height / 2 - lines.Count / 2;
                lineNum = i < 0 ? 0 : i;
            } else if(vert == DirectionalConstants.BOTTOM) {
                int i = size.Height - lines.Count;
                lineNum = i < 0 ? 0 : i;
            }
            foreach(String line in lines) {
                if(lineNum >= size.Height) break;
                int colNum = 0;
                if(horiz == DirectionalConstants.LEFT) {
                    colNum = 0;
                } else if(horiz == DirectionalConstants.CENTER) {
                    int i = size.Width / 2 - line.Length / 2;
                    colNum = i < 0 ? 0 : i;
                } else if(horiz == DirectionalConstants.RIGHT) {
                    int i = size.Width - line.Length;
                    colNum = i < 0 ? 0 : i;
                }
                String l = line;
                if(line.Length > size.Width) l = line.Substring(0, size.Width);
                g.DrawText(colNum, lineNum++, l);
            }
            
        }

        public override Size GetPreferredSize() {
            List<String> lines = text.Split("\n").OfType<String>().ToList();
            return new Size(lines.Max(e => e.Length), lines.Count);
        }

    }

}

