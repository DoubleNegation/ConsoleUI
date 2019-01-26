using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ConsoleUI {

    public class Component {

        private ILayoutManager layoutManager;
        private ILayoutManager parentLayout;

        private Color foreground = null;
        private Color background = null;

        public Component() {
            Layout = new BorderLayout();
        }

        public Component(ILayoutManager layoutManager) {
            Layout = layoutManager;
        }

        public virtual void PaintComponent(Graphics g) {
            
        }

        public virtual void PaintComponents(Graphics g) {
            PaintComponent(g);
            if(layoutManager == null) return;
            Size mySize = GetSize();
            foreach(Component c in layoutManager.GetComponents()) {
                Size size = layoutManager.GetSizeOf(c);
                if(size.Width <= 0 || size.Height <= 0) continue;
                Point location = layoutManager.GetLocationOf(c);
                g.Translate(location.X, location.Y, -(mySize.Width-size.Width) + location.X, -(mySize.Height-size.Height) + location.Y);
                c.PaintComponents(g);
                g.Translate(-location.X, -location.Y, mySize.Width-size.Width - location.X, mySize.Height-size.Height - location.Y);
            }
        }

        public virtual Size GetPreferredSize() {
            return layoutManager.GetPreferredSize();
        }

        public virtual bool IsInteractive() {
            return false;
        }

        public virtual ILayoutManager Layout {
            get {
                return layoutManager;
            } set {
                layoutManager = value;
                if(layoutManager != null)
                    layoutManager.OnAddedToComponent(this);
            }
        }

        public virtual ILayoutManager ParentLayout {
            set {
                parentLayout = value;
            }
        }

        public virtual Color Foreground {
            get {
                return foreground;
            } set {
                foreground = value;
            }
        }

        public virtual Color Background {
            get {
                return background;
            } set {
                background = value;
            }
        }

        public virtual Component[] GetComponents() {
            return layoutManager.GetComponents();
        }

        public virtual void Add(Component c) {
            layoutManager.Add(c);
        }

        public virtual void Add(Component c, Object constraints) {
            layoutManager.Add(c, constraints);
        }

        public virtual void Remove(Component c) {
            layoutManager.Remove(c);
        }

        public virtual void RemoveAll() {
            layoutManager.RemoveAll();
        }

        public virtual Size GetSize() {
            return parentLayout.GetSizeOf(this);
        }

        public virtual int GetWidth() {
            return parentLayout.GetSizeOf(this).Width;
        }

        public virtual int GetHeight() {
            return parentLayout.GetSizeOf(this).Height;
        }

        public virtual Component Parent {
            get {
                return parentLayout.GetOwner();
            }
        }

        public virtual Point GetLocationInWindow() {
            Window root = GetWindow();
            List<Component> componentTree = new List<Component>{this};
            while(componentTree.Last() != root) {
                componentTree.Add(componentTree.Last().Parent);
            }
            int xSum = 0;
            int ySum = 0;
            foreach(Component c in componentTree) {
                if(c == root) continue;
                Point p = c.parentLayout.GetLocationOf(c);
                xSum += p.X;
                ySum += p.Y;
            }
            return new Point(xSum, ySum);
        }

        public virtual Point GetLocationRelativeTo(Component c) {
            Point myLoc = GetLocationInWindow();
            Point otherLoc = c.GetLocationInWindow();
            return new Point(otherLoc.X - myLoc.Y, otherLoc.Y - myLoc.Y);
        }

        public virtual Window GetWindow() {
            return parentLayout.GetWindow();
        }

        public virtual void Invalidate() {
            layoutManager.Invalidate();
        }

        public virtual void Validate() {
            layoutManager.Validate();
        }

        public virtual void Revalidate() {
            Invalidate();
            Validate();
        }

        /* ---------------- */
        /* EVENT MANAGEMENT */
        /* ---------------- */

        private List<IActionListener> actionListeners = new List<IActionListener>();
        private List<IFocusListener> focusListeners = new List<IFocusListener>();
        private List<IKeyListener> keyListeners = new List<IKeyListener>();

        public virtual void DispatchEvent(Event ev) {
            if(ev is ActionEvent) {
                ActionEvent e = (ActionEvent) ev;
                foreach(IActionListener actionListener in actionListeners) {
                    actionListener.ActionPerformed(e);
                }
            } else if(ev is FocusEvent) {
                FocusEvent e = (FocusEvent) ev;
                if(e.Type == FocusEvent.FOCUS_GAINED) {
                    foreach(IFocusListener focusListener in focusListeners) {
                        focusListener.FocusGained(e);
                    }
                } else if(e.Type == FocusEvent.FOCUS_LOST) {
                    foreach(IFocusListener focusListener in focusListeners) {
                        focusListener.FocusLost(e);
                    }
                }
            } else if(ev is KeyEvent) {
                KeyEvent e = (KeyEvent) ev;
                foreach(IKeyListener keyListener in keyListeners) {
                    keyListener.KeyTyped(e);
                }
            }
        }

        public void AddActionListener(IActionListener listener) {
            actionListeners.Add(listener);
        }

        public void AddFocusListener(IFocusListener listener) {
            focusListeners.Add(listener);
        }

        public void AddKeyListener(IKeyListener listener) {
            keyListeners.Add(listener);
        }

        public void RemoveActionListener(IActionListener listener) {
            actionListeners.Remove(listener);
        }

        public void RemoveFocusListener(IFocusListener listener) {
            focusListeners.Remove(listener);
        }

        public void RemoveKeyListener(IKeyListener listener) {
            keyListeners.Remove(listener);
        }

        public void RemoveActionListener(int index) {
            actionListeners.RemoveAt(index);
        }

        public void RemoveFocusListener(int index) {
            focusListeners.RemoveAt(index);
        }

        public void RemoveKeyListener(int index) {
            keyListeners.RemoveAt(index);
        }

        public IActionListener[] getActionListeners() {
            return actionListeners.ToArray();
        }

        public IFocusListener[] getFocusListeners() {
            return focusListeners.ToArray();
        }

        public IKeyListener[] getKeyListeners() {
            return keyListeners.ToArray();
        }

    }

}

