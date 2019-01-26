using System;
using System.Drawing;
using System.Text;

namespace ConsoleUI {

    public class Button : Component {

        private String text;
        private bool focused = false;
        private Label borderTop;
        private Label borderRight;
        private Label borderBottom;
        private Label borderLeft;
        private Label textLabel;

        public Button(String text) : base(new BorderLayout()) {
            borderTop = new Label("");
            borderRight = new Label("");
            borderBottom = new Label("");
            borderLeft = new Label("");
            textLabel = new Label("");
            textLabel.HorizontalAlignment = DirectionalConstants.CENTER;
            Add(borderTop, DirectionalConstants.TOP);
            Add(borderRight, DirectionalConstants.RIGHT);
            Add(borderBottom, DirectionalConstants.BOTTOM);
            Add(borderLeft, DirectionalConstants.LEFT);
            Add(textLabel, DirectionalConstants.CENTER);
            Text = text;
            AddFocusListener(new ButtonFocusListener(this));
        }

        public String Text {
            get {
                return text;
            } set {
                text = value;
                textLabel.Text = text;
                Invalidate();
            }
        }

        public override void PaintComponent(Graphics g) {
            Color bg = Background;
            bg = bg == null ? Color.LIGHT_BLUE : bg;
            borderTop.Background = bg;
            borderRight.Background = bg;
            borderBottom.Background = bg;
            borderLeft.Background = bg;
            textLabel.Background = bg;
            Color fg = Color.BLACK;
            if(focused) {
                fg = Color.WHITE;
            } else {
                Color c = Foreground;
                if(c != null) {
                    fg = c;
                }
            }
            borderTop.Foreground = fg;
            borderRight.Foreground = fg;
            borderBottom.Foreground = fg;
            borderLeft.Foreground = fg;
            textLabel.Foreground = fg;
        }

        public override Size GetPreferredSize() {
            Size textSize = textLabel.GetPreferredSize();
            return new Size(textSize.Width + 4, textSize.Height + 2);
        }

        public override bool IsInteractive() {
            return true;
        }

        public override void Validate() {
            Size size = GetSize();
            StringBuilder horiz = new StringBuilder("+");
            for(int i = 0; i < size.Width - 2; i++) {
                horiz.Append("-");
            }
            horiz.Append("+");
            String h = horiz.ToString();
            StringBuilder vert = new StringBuilder("");
            for(int i = 0; i < size.Height - 2; i++) {
                vert.Append("|");
                vert.Append("\n");
            }
            String v = vert.ToString();
            borderTop.Text = h;
            borderRight.Text = v;
            borderBottom.Text = h;
            borderLeft.Text = v;
            base.Validate();
        }

        private class ButtonFocusListener : IFocusListener {
            private Button target;
            public ButtonFocusListener(Button target) {
                this.target = target;
            }
            public void FocusGained(FocusEvent e) {
                if(e.Source == target) {
                    target.focused = true;
                    target.GetWindow().PaintLater();
                }
            }
            public void FocusLost(FocusEvent e) {
                if(e.Source == target) {
                    target.focused = false;
                    target.GetWindow().PaintLater();
                }
            }
        }

    }

}

