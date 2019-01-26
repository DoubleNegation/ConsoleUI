using System;

namespace ConsoleUI {

    public class Event {

        public Event(Component source) {
            this.source = source;
        }

        private Component source;
        private bool consumed = false;

        public bool Consumed {
            get {
                return consumed;
            } set {
                consumed = value;
            }
        }

        public Component Source {
            get {
                return source;
            }
        }

        /// <summary>
        /// Identical to Consumed = true
        /// </summary>
        public void Consume() {
            Consumed = true;
        }

    }

}

