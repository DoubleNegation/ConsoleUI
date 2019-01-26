using System;

namespace ConsoleUI {

    public class FocusEvent : Event {

        public static readonly int FOCUS_GAINED = 0;
        public static readonly int FOCUS_LOST = 1;

        private int type;

        public FocusEvent(Component source, int type) : base(source) {
            if(type != FOCUS_GAINED && type != FOCUS_LOST) throw new ArgumentException("Invalid Type");
            this.type = type;
        }

        public int Type {
            get {
                return type;
            }
        }

    }

    public interface IFocusListener {
        void FocusGained(FocusEvent e);
        void FocusLost(FocusEvent e);
    }

    public class FocusAdapter : IFocusListener {
        public delegate void FocusEventMethod(FocusEvent e);
        private FocusEventMethod gain;
        private FocusEventMethod loose;
        public void FocusGained(FocusEvent e) {
            gain(e);
        }
        public void FocusLost(FocusEvent e) {
            loose(e);
        }
        public FocusAdapter FocusGained(FocusEventMethod m) {
            gain = m;
            return this;
        }
        public FocusAdapter FocusLost(FocusEventMethod m) {
            loose = m;
            return this;
        }
    }

}

