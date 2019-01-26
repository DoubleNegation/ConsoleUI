using System;
using System.Drawing;

namespace ConsoleUI {

    public class ProgressBar : Component {

        private int min = 0;
        private int max = 100;
        private int progress = 0;
        private Label label = new Label("");
        int prefWidth = 30;
        int prefHeight = 1;

        public ProgressBar() : base(new BorderLayout()) {
            label.HorizontalAlignment = DirectionalConstants.CENTER;
            label.Foreground = Color.BLACK;
            Add(label, DirectionalConstants.CENTER);
        }

        public ProgressBar(int min, int max) : base(new BorderLayout()) {
            label.HorizontalAlignment = DirectionalConstants.CENTER;
            label.Foreground = Color.BLACK;
            Add(label, DirectionalConstants.CENTER);
            Minimum = min;
            Maximum = max;
        }

        public ProgressBar(int min, int max, int progress, String text) : base(new BorderLayout()) {
            label.HorizontalAlignment = DirectionalConstants.CENTER;
            label.Foreground = Color.BLACK;
            Add(label, DirectionalConstants.CENTER);
            Minimum = min;
            Maximum = max;
            Progress = progress;
            Text = text;
        }

        public virtual int Minimum {
            get {
                return min;
            } set {
                min = value;
                try {
                    GetWindow().PaintLater();
                } catch(NullReferenceException) {}
            }
        }

        public virtual int Maximum {
            get {
                return max;
            } set {
                max = value;
                try {
                    GetWindow().PaintLater();
                } catch(NullReferenceException) {}
            }
        }

        public virtual int Progress {
            get {
                return progress;
            } set {
                progress = value;
                try {
                    GetWindow().PaintLater();
                } catch(NullReferenceException) {}
            }
        }

        public virtual String Text {
            get {
                return label.Text;
            } set {
                label.Text = value;
                try {
                    GetWindow().PaintLater();
                } catch(NullReferenceException) {}
            }
        }

        public virtual int PreferredWidth {
            get {
                return prefWidth;
            } set {
                if(value <= 0) throw new ArgumentOutOfRangeException("PreferredWidth must be greater than 0.");
                prefWidth = value;
                Invalidate();
            }
        }

        public virtual int PreferredHeight {
            get {
                return prefHeight;
            } set {
                if(value <= 0) throw new ArgumentOutOfRangeException("PreferredHeight must be greater than 0.");
                prefHeight = value;
                Invalidate();
            }
        }

        public override void PaintComponents(Graphics g) {
            base.PaintComponents(g);
            Size size = GetSize();
            int paint = (size.Width * (progress - min)) / (max - min);
            Color active = Foreground;
            Color inactive = Background;
            active = active == null ? Color.YELLOW : active;
            inactive = inactive == null ? Color.LIGHT_BLUE : inactive;
            g.Color = active;
            g.FillBackgroundRect(0, 0, paint, size.Height);
            g.Color = inactive;
            g.FillBackgroundRect(paint, 0, size.Width - paint, size.Height);
        }

        public override Size GetPreferredSize() {
            return new Size(prefWidth, prefHeight);
        }

    }

}

