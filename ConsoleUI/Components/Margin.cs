using System;
using System.Drawing;

namespace ConsoleUI {

    public class Margin : Component {

        private Component content;
        private int top;
        private int right;
        private int bottom;
        private int left;
        private MarginSideContent borderTop;
        private MarginSideContent borderRight;
        private MarginSideContent borderBottom;
        private MarginSideContent borderLeft;

        public Margin(Component content) : base(new BorderLayout()) {
            Content = content;
            Init();
            Top = 0;
            Right = 0;
            Bottom = 0;
            Left = 0;
        }

        public Margin(Component content, int margins) : base(new BorderLayout()) {
            Content = content;
            Init();
            Top = margins;
            Right = margins;
            Bottom = margins;
            Left = margins;
        }

        public Margin(Component content, int vertical, int horizontal) : base(new BorderLayout()) {
            Content = content;
            Init();
            Top = vertical;
            Right = horizontal;
            Bottom = vertical;
            Left = horizontal;
        }

        public Margin(Component content, int top, int right, int bottom, int left) : base(new BorderLayout()) {
            Content = content;
            Init();
            Top = top;
            Right = right;
            Bottom = bottom;
            Left = left;
        }

        private void Init() {
            borderTop = new MarginSideContent(DirectionalConstants.VERTICAL, top);
            borderRight = new MarginSideContent(DirectionalConstants.HORIZONTAL, right);
            borderBottom = new MarginSideContent(DirectionalConstants.VERTICAL, bottom);
            borderLeft = new MarginSideContent(DirectionalConstants.HORIZONTAL, left);
            Add(borderTop, DirectionalConstants.TOP);
            Add(borderRight, DirectionalConstants.RIGHT);
            Add(borderBottom, DirectionalConstants.BOTTOM);
            Add(borderLeft, DirectionalConstants.LEFT);
            Add(content, DirectionalConstants.CENTER);
        }

        public Component Content {
            get {
                return content;
            } set {
                if(value == null) throw new NullReferenceException("Margin content can not be null");
                Remove(content);
                content = value;
                Add(content, DirectionalConstants.CENTER);
                Invalidate();
            }
        }

        public int Top {
            get {
                return top;
            } set {
                if(value < 0) throw new ArgumentOutOfRangeException("Margin may not be smaller than 0.");
                top = value;
                borderTop.Size = value;
                Invalidate();
            }
        }

        public int Right {
            get {
                return right;
            } set {
                if(value < 0) throw new ArgumentOutOfRangeException("Margin may not be smaller than 0.");
                right = value;
                borderRight.Size = value;
                Invalidate();
            }
        }

        public int Bottom {
            get {
                return bottom;
            } set {
                if(value < 0) throw new ArgumentOutOfRangeException("Margin may not be smaller than 0.");
                bottom = value;
                borderBottom.Size = value;
                Invalidate();
            }
        }

        public int Left {
            get {
                return left;
            } set {
                if(value < 0) throw new ArgumentOutOfRangeException("Margin may not be smaller than 0.");
                left = value;
                borderLeft.Size = value;
                Invalidate();
            }
        }

        public class MarginSideContent : Component {
            private readonly DirectionalConstants orientation;
            private int size;
            public MarginSideContent(DirectionalConstants orientation, int size) : base(new BorderLayout()) {
                if(orientation != DirectionalConstants.HORIZONTAL && orientation != DirectionalConstants.VERTICAL) {
                    throw new ArgumentException("The specified side is not a valid side.");
                }
                this.orientation = orientation;
                Size = size;
            }
            public int Size {
                get {
                    return size;
                } set {
                    if(value < 0) {
                        throw new ArgumentOutOfRangeException("Margin may not be smaller than 0.");
                    }
                    size = value;
                    Invalidate();
                }
            }
            public override Size GetPreferredSize() {
                if(orientation == DirectionalConstants.HORIZONTAL) {
                    return new Size(size, 0);
                } else {
                    return new Size(0, size);
                }
            }
        }

    }

}

