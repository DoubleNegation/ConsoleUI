using System;

namespace ConsoleUI {

    public class KeyEvent : Event {

        private ConsoleKeyInfo key;

        public KeyEvent(Component source, ConsoleKeyInfo key) : base(source) {
            this.key = key;
        }

        public ConsoleKeyInfo Key {
            get {
                return key;
            }
        }

    }

    public interface IKeyListener {
        void KeyTyped(KeyEvent e);
    }

    public class KeyAdapter : IKeyListener {
        public delegate void KeyEventMethod(KeyEvent e);
        private KeyEventMethod keyTyped;
        public void KeyTyped(KeyEvent e) {
            keyTyped(e);
        }
        public KeyAdapter KeyTyped(KeyEventMethod m) {
            keyTyped = m;
            return this;
        }
    }

}

