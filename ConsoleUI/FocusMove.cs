using System;
using System.Collections.Generic;

namespace ConsoleUI {

    //TODO: always validate if a component is removed from the component tree
    public class FocusMove {
        
        private Window window;
        private Component focusedComponent;

        /// <summary>
        /// The component that is currently focused in the application
        /// </summary>
        public Component FocusedComponent {
            get {
                return focusedComponent;
            } set {
                if(Validate(value)) {
                    if(focusedComponent != null) {
                        //focus lost event
                        window.EnqueueEvent(new FocusEvent(focusedComponent, FocusEvent.FOCUS_LOST));
                    }
                    focusedComponent = value;
                    //focus gained event
                    window.EnqueueEvent(new FocusEvent(focusedComponent, FocusEvent.FOCUS_GAINED));
                }
            }
        }

        public FocusMove(Window window) {
            this.window = window;
            FocusedComponent = FindFirstActivatable();
        }

        public FocusMove(Window window, Component comp) {
            this.window = window;
            FocusedComponent = comp;
            if(focusedComponent != comp) {
                FocusedComponent = FindFirstActivatable();
            }
        }

        /// <summary>
        /// Checks if the specified component is a child of the window that was specified in the constructor.
        /// </summary>
        private bool Validate(Component comp) {
            if(comp == window) return true;
            Stack<Component> comps = new Stack<Component>();
            comps.Push(window);
            while(comps.Count > 0) {
                Component c = comps.Pop();
                Component[] cs = c.GetComponents();
                foreach(Component co in cs) {
                    if(co == comp) return true;
                    comps.Push(co);
                }
            }
            return false;
        }

        /// <summary>
        /// Finds the first "activatable" component in the window that was specified in the constructor.
        /// That component is used for initially placing the cursor.
        /// </summary>
        private Component FindFirstActivatable() {
            Stack<Component> comps = new Stack<Component>();
            comps.Push(window);
            while(comps.Count > 0) {
                Component c = comps.Pop();
                Component[] cs = c.GetComponents();
                foreach(Component co in cs) {
                    if(co.IsInteractive()) return co;
                    comps.Push(co);
                }
            }
            return null;
        }

        private Stack<Component> Locate(Component c) {
            Stack<Component> location = new Stack<Component>();
            List<Component> list = Locate(c, window);
            for(int i = list.Count - 1; i >= 0; i--) {
                location.Push(list[i]);
            }
            return location;
        }

        private List<Component> Locate(Component c, Component at) {
            foreach(Component comp in at.GetComponents()) {
                if(comp == c) return new List<Component>{c, at};
                List<Component> passthru = Locate(c, comp);
                if(passthru == null) continue;
                passthru.Add(at);
                return passthru;
            }
            return null;
        }

        /// <summary>
        /// Moves focus to the next component in an upwards direction.
        /// </summary>
        public Component Up() {
            //find the component
            Component result = null;
            Component current = focusedComponent;
            while (result == null && current != window) {
                result = _Up(current);
                if(current == window) break;
                current = current.Parent;
            }
            //update focused component
            if(result != null) {
                if(focusedComponent != null) {
                    //focus lost event
                    window.EnqueueEvent(new FocusEvent(focusedComponent, FocusEvent.FOCUS_LOST));
                }
                focusedComponent = result;
                //focus gained event
                window.EnqueueEvent(new FocusEvent(focusedComponent, FocusEvent.FOCUS_GAINED));
            }
            return result;
        }

        /// <summary>
        /// Finds the bottom-most focusable component immediately above c (= with the same parent)
        /// Returns null if there is none.
        /// </summary>
        private Component _Up(Component c) {
            Component[] arr = c.Parent.Layout.GetComponentsAbove(c);
            foreach(Component comp in arr) {
                Component result = __Up(comp);
                if(result != null) return result;
            }
            if(c.Parent.IsInteractive()) return c.Parent;
            return null;
        }

        /// </summary>
        /// Finds the bottom-most activatable component inside c.
        /// Returns null if there is none.
        /// </summary>
        private Component __Up(Component c) {
            Component[] arr = ConsoleUtils.CopyAndFlipArray(c.Layout.OrderedComponentsTTB());
            foreach(Component comp in arr) {
                Component result = __Up(comp);
                if(result != null) return result;
            }
            if(c.IsInteractive()) return c;
            return null;
        }

        /// <summary>
        /// Moves focus to the next component in a downwards direction.
        /// </summary>
        public Component Down() {
            //find the component
            Component result = null;
            Component current = focusedComponent;
            while (result == null && current != window) {
                result = _Down(current);
                if(current == window) break;
                current = current.Parent;
            }
            //update focused component
            if(result != null) {
                if(focusedComponent != null) {
                    //focus lost event
                    window.EnqueueEvent(new FocusEvent(focusedComponent, FocusEvent.FOCUS_LOST));
                }
                focusedComponent = result;
                //focus gained event
                window.EnqueueEvent(new FocusEvent(focusedComponent, FocusEvent.FOCUS_GAINED));
            }
            return result;
        }
        
        /// <summary>
        /// Finds the top-most focusable component immediately below c (= with the same parent)
        /// Returns null if there is none.
        /// </summary>
        private Component _Down(Component c) {
            Component[] arr = c.Parent.Layout.GetComponentsBelow(c);
            foreach(Component comp in arr) {
                Component result = __Down(comp);
                if(result != null) return result;
            }
            if(c.Parent.IsInteractive()) return c.Parent;
            return null;
        }

        /// </summary>
        /// Finds the top-most activatable component inside c.
        /// Returns null if there is none.
        /// </summary>
        private Component __Down(Component c) {
            if(c.IsInteractive()) return c;
            Component[] arr = c.Layout.OrderedComponentsTTB();
            foreach(Component comp in arr) {
                Component result = __Down(comp);
                if(result != null) return result;
            }
            return null;
        }

        /// <summary>
        /// Moves focus to the next component in a leftwards diretion
        /// </summary>
        public Component Left() {
            //find the component
            Component result = null;
            Component current = focusedComponent;
            while(result == null && current != window) {
                result = _Left(current);
                if(current == window) break;
                current = current.Parent;
            }
            //update focused component
            if(result != null) {
                if(focusedComponent != null) {
                   window.EnqueueEvent(new FocusEvent(focusedComponent, FocusEvent.FOCUS_LOST));
                }
                focusedComponent = result;
                window.EnqueueEvent(new FocusEvent(focusedComponent, FocusEvent.FOCUS_GAINED));
            }
            return result;
        }

        /// <summary>
        /// Finds the bottom-most focusable component immediately left to c (= with the same parent)
        /// Returns null if there is none.
        /// </summary>
        private Component _Left(Component c) {
            Component[] arr = c.Parent.Layout.GetComponentsLeftTo(c);
            foreach(Component comp in arr) {
                Component result = __Left(comp);
                if(result != null) return result;
            }
            if(c.Parent.IsInteractive()) return c.Parent;
            return null;
        }

        /// <summary>
        /// Finds the right-most activatable component inside c.
        /// Returns null if there is none.
        /// </summary>
        private Component __Left(Component c) {
            Component[] arr = ConsoleUtils.CopyAndFlipArray(c.Layout.OrderedComponentsLTR());
            foreach(Component comp in arr) {
                Component result = __Up(comp);
                if(result != null) return result;
            }
            if(c.IsInteractive()) return c;
            return null;
        }

        /// <summary>
        /// Moves focus to the next component in a rightwards diretion
        /// </summary>
        public Component Right() {
            //find the component
            Component result = null;
            Component current = focusedComponent;
            while(result == null && current != window) {
                result = _Right(current);
                if(current == window) break;
                current = current.Parent;
            }
            //update focused component
            if(result != null) {
                if(focusedComponent != null) {
                    //focus lost event
                    window.EnqueueEvent(new FocusEvent(focusedComponent, FocusEvent.FOCUS_LOST));
                }
                focusedComponent = result;
                //focus gained event
                window.EnqueueEvent(new FocusEvent(focusedComponent, FocusEvent.FOCUS_GAINED));
            }
            return result;
        }

        /// <summary>
        /// Finds the bottom-most focusable component immediately right to c (= with the same parent)
        /// Returns null if there is none.
        /// </summary>
        private Component _Right(Component c) {
            Component[] arr = c.Parent.Layout.GetComponentsRightTo(c);
            foreach(Component comp in arr) {
                Component result = __Right(comp);
                if(result != null) return result;
            }
            if(c.Parent.IsInteractive()) return c.Parent;
            return null;
        }

        /// <summary>
        /// Finds the left-most activatable component inside c.
        /// Returns null if there is none.
        /// </summary>
        private Component __Right(Component c) {
            if(c.IsInteractive()) return c;
            Component[] arr = c.Layout.OrderedComponentsLTR();
            foreach(Component comp in arr) {
                Component result = __Right(comp);
                if(result != null) return result;
            }
            return null;
        }

    }

}

