using System;
using System.Drawing;
using System.Text;

namespace ConsoleUI {

    class TextField : Component {

        private String text;
        private int caretLocation;
        private int preferredWidth = 20;
        private Label borderTop;
        private Label borderRight;
        private Label borderBottom;
        private Label borderLeft;
        private bool focused = false;
        private bool active = false;
        private int drawOffset = 0;

        public TextField() : base(new BorderLayout()) {
            text = "";
            caretLocation = 0;
            setUpCompTree();
        }

        public TextField(String initialText) : base(new BorderLayout()) {
            if(initialText == null) throw new NullReferenceException("The text may not be null.");
            text = initialText;
            caretLocation = text.Length;
            setUpCompTree();
        }

        private void setUpCompTree() {
            borderTop = new Label("");
            borderRight = new Label("");
            borderBottom = new Label("");
            borderLeft = new Label("");
            Add(borderTop, DirectionalConstants.TOP);
            Add(borderRight, DirectionalConstants.RIGHT);
            Add(borderBottom, DirectionalConstants.BOTTOM);
            Add(borderLeft, DirectionalConstants.LEFT);
            AddActionListener(new ActionAdapter().ActionPerformed(OnActionPerformed));
            AddFocusListener(new FocusAdapter().FocusGained(OnFocusGained).FocusLost(OnFocusLost));
            AddKeyListener(new KeyAdapter().KeyTyped(OnKeyTyped));
        }

        public String Text {
            get {
                return text;
            } set {
                if(value == null) throw new NullReferenceException("The text may not be null.");
                text = value;
                if(caretLocation > text.Length) {
                    caretLocation = text.Length;
                    drawOffset = Math.Max(caretLocation - GetSize().Width + 3, 0);
                }
            }
        }

        public int CaretPosition {
            get {
                return caretLocation;
            } set {
                if(value < 0 || value > text.Length) throw new ArgumentOutOfRangeException("The caret location is not in the range of the text.");
                caretLocation = value;
            }
        }

        public int PreferredWidth {
            get {
                return preferredWidth - 2;
            } set {
                if(value <= 1) throw new ArgumentException("pref width too small");
                preferredWidth = value + 2;
            }
        }

        public bool Active {
            get {
                return active;
            }
        }

        public override void PaintComponent(Graphics g) {
            Color bg = Background;
            bg = bg == null ? Color.BLUE : bg;
            Color fg = null;
            if(focused) {
                fg = Color.WHITE;
            } else {
                fg = Foreground;
                fg = fg == null ? Color.YELLOW : fg;
            }
            borderTop.Background = bg;
            borderRight.Background = bg;
            borderBottom.Background = bg;
            borderLeft.Background = bg;
            borderTop.Foreground = fg;
            borderRight.Foreground = fg;
            borderBottom.Foreground = fg;
            borderLeft.Foreground = fg;
            g.Color = bg;
            Size s = GetSize();
            int w = s.Width - 4;
            g.FillBackgroundRect(1, 1, w + 2, 1);
            g.Color = fg;
            if(active) {
                String drawText = text.Substring(drawOffset, Math.Min(w, text.Length - drawOffset));
                g.DrawText(2, 1, drawText);
                int cursor = caretLocation - drawOffset;
                g.FillBackgroundRect(2 + cursor, 1, 1, 1);
                if(caretLocation != text.Length) {
                    g.Color = bg;
                    g.DrawText(2 + cursor, 1, drawText.Substring(cursor, 1));
                }
            } else {
                String drawText = text.Substring(0, Math.Min(text.Length, w));
                g.DrawText(2, 1, drawText);
            }
        }

        public override Size GetPreferredSize() {
            return new Size(preferredWidth, 3);
        }

        public override bool IsInteractive() {
            return true;
        }

        public override void Validate() {
            Size size = GetSize();
            StringBuilder top = new StringBuilder("█");
            for(int i = 0; i < size.Width - 2; i++) {
                top.Append("▀");
            }
            top.Append("█");
            String t = top.ToString();
            StringBuilder right = new StringBuilder("");
            for(int i = 0; i < size.Height - 2; i++) {
                right.Append("█");
                right.Append("\n");
            }
            String r = right.ToString();
            StringBuilder bottom = new StringBuilder("█");
            for(int i = 0; i < size.Width - 2; i++) {
                bottom.Append("▄");
            }
            bottom.Append("█");
            String b = bottom.ToString();
            StringBuilder left = new StringBuilder("");
            for(int i = 0; i < size.Height - 2; i++) {
                left.Append("█");
                left.Append("\n");
            }
            String l = left.ToString();
            borderTop.Text = t;
            borderRight.Text = r;
            borderBottom.Text = b;
            borderLeft.Text = l;
            base.Validate();
        }

        private void OnActionPerformed(ActionEvent e) {
            active = !active;
            if(active) {
                caretLocation = text.Length;
                drawOffset = Math.Max(caretLocation - GetSize().Width + 5, 0);
            }
            GetWindow().PaintLater();
        }

        private void OnFocusGained(FocusEvent e) {
            focused = true;
            GetWindow().PaintLater();
        }

        private void OnFocusLost(FocusEvent e) {
            focused = false;
            GetWindow().PaintLater();
        }

        private void OnKeyTyped(KeyEvent e) {
            if(active) {
                e.Consume();
                if(e.Key.Key == ConsoleKey.Enter) {
                    GetWindow().EnqueueEvent(new ActionEvent(e.Source));
                } else if(e.Key.Key == ConsoleKey.LeftArrow) {
                    if(caretLocation > 0) {
                        caretLocation--;
                        GetWindow().PaintLater();
                    }
                    if(1 + (caretLocation - drawOffset) <= 0) {
                        drawOffset--;
                    }
                } else if(e.Key.Key == ConsoleKey.RightArrow) {
                    if(caretLocation < text.Length) {
                        caretLocation++;
                        GetWindow().PaintLater();
                    }
                    if(caretLocation - drawOffset >= GetSize().Width - 4) {
                        drawOffset++;
                    }
                } else if(e.Key.Key == ConsoleKey.Backspace) {
                    if(caretLocation != 0) {
                        text = text.Substring(0, caretLocation - 1) + text.Substring(caretLocation, text.Length - caretLocation);
                        caretLocation--;
                        if(caretLocation - drawOffset < 0) {
                            drawOffset = caretLocation;
                        }
                        if(text.Length - drawOffset < GetSize().Width && drawOffset > 0) {
                            drawOffset--;
                        }
                        GetWindow().PaintLater();
                    }
                } else if(e.Key.Key == ConsoleKey.Delete) {
                    if(caretLocation != text.Length) {
                        text = text.Substring(0, caretLocation) + text.Substring(caretLocation + 1, text.Length - caretLocation - 1);
                        GetWindow().PaintLater();
                    }
                } else if(e.Key.Key == ConsoleKey.Home) {
                    caretLocation = 0;
                    drawOffset = 0;
                    GetWindow().PaintLater();
                } else if(e.Key.Key == ConsoleKey.End) {
                    caretLocation = text.Length;
                    drawOffset = Math.Max(caretLocation - GetSize().Width + 5, 0);
                    GetWindow().PaintLater();
                } else {
                    if(e.Key.KeyChar != 0 && (e.Key.Modifiers & ConsoleModifiers.Control) == 0) {
                        text = text.Substring(0, caretLocation) + e.Key.KeyChar + text.Substring(caretLocation, text.Length - caretLocation);
                        caretLocation++;
                        if(caretLocation - drawOffset > GetSize().Width - 5) {
                            drawOffset++;
                        }
                        GetWindow().PaintLater();
                    }
                }
            }
        }

    }

}

