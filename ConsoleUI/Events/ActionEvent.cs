using System;

namespace ConsoleUI{

    public class ActionEvent : Event {

        public ActionEvent(Component source) : base(source) {

        }
    
    }

    public interface IActionListener {
        void ActionPerformed(ActionEvent e);
    }

    public class ActionAdapter : IActionListener {
        public delegate void ActionEventMethod(ActionEvent e);
        private ActionEventMethod action;
        public void ActionPerformed(ActionEvent e) {
            action(e);
        }
        public ActionAdapter ActionPerformed(ActionEventMethod m) {
            action = m;
            return this;
        }
    }

}

