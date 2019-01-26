using System;
using System.Collections.Generic;
using System.Drawing;

namespace ConsoleUI {

   public class StackLayout : ILayoutManager {

        private Component owner = null;
        private DirectionalConstants orientation;
        private List<Component> comps = new List<Component>();
        private List<int> calculatedLengths = new List<int>();
        private int pref = 0;

        public StackLayout() {
            orientation = DirectionalConstants.VERTICAL;
        }

        public StackLayout(DirectionalConstants orientation) {
            Orientation = orientation;
        }

        public DirectionalConstants Orientation {
            get {
                return orientation;
            } set {
                if(value == DirectionalConstants.HORIZONTAL || value == DirectionalConstants.VERTICAL) {
                    orientation = value;
                } else {
                    throw new ArgumentException("Invalid orientation.");
                }
            }
        }

        public virtual void Invalidate() {
            foreach(Component comp in comps) {
                comp.Invalidate();
            }
        }

        public virtual void Validate() {
            Size availableSpace = owner.GetSize();
            int ah = availableSpace.Height;
            int aw = availableSpace.Width;
            int relevantLength = orientation == DirectionalConstants.HORIZONTAL ? aw : ah;
            List<int> lengths = new List<int>();
            int sumLengths = 0;
            foreach(Component c in comps) {
                Size pref = c.GetPreferredSize();
                int rel = orientation == DirectionalConstants.HORIZONTAL ? pref.Width : pref.Height;
                lengths.Add(rel);
                sumLengths += rel;
                int nrel = orientation == DirectionalConstants.HORIZONTAL ? pref.Height : pref.Width;
                if(nrel > this.pref) this.pref = nrel;
            }
            if(sumLengths <= relevantLength) {
                calculatedLengths = lengths;
            } else {
                int remaining = relevantLength;
                calculatedLengths.Clear();
                foreach(int len in lengths) {
                    if(len <= remaining) {
                        calculatedLengths.Add(len);
                        remaining -= len;
                    } else {
                        calculatedLengths.Add(remaining);
                        remaining = 0;
                    }
                }
            }
            foreach(Component comp in comps) {
                comp.Validate();
            }
        }

        public virtual Component[] GetComponents() {
            return comps.ToArray();
        }

        public virtual void Add(Component c) {
            comps.Add(c);
            Invalidate();
            c.ParentLayout = this;
        }

        public virtual void Add(Component c, Object constraints) {
            Add(c);
        }

        public virtual void Remove(Component c) {
            if(comps.Contains(c)) {
                comps.Remove(c);
                Invalidate();
            }
        }

        public virtual void RemoveAll() {
            comps.Clear();
            Invalidate();
        }

        public virtual Size GetSizeOf(Component c) {
            if(orientation == DirectionalConstants.HORIZONTAL) {
                return new Size(calculatedLengths[comps.IndexOf(c)], owner.GetSize().Height);
            } else {
                return new Size(owner.GetSize().Width, calculatedLengths[comps.IndexOf(c)]);
            }
        }

        public virtual Point GetLocationOf(Component c) {
            int sum = 0;
            for(int i = 0; i < comps.IndexOf(c); i++) {
                sum += calculatedLengths[i];
            }
            if(orientation == DirectionalConstants.HORIZONTAL) {
                return new Point(sum, 0);
            } else {
                return new Point(0, sum);
            }
        }

        public virtual Window GetWindow() {
            return owner.GetWindow();
        }

        public virtual void OnAddedToComponent(Component c) {
            if(owner != null) {
                owner.Layout = null;
            }
            owner = c;
        }

        public virtual Size GetPreferredSize() {
            int sum = 0;
            int max = 0;
            foreach(Component comp in comps) {
                Size s = comp.GetPreferredSize();
                sum += orientation == DirectionalConstants.HORIZONTAL ? s.Width : s.Height;
                int rel = orientation == DirectionalConstants.HORIZONTAL ? s.Height : s.Width;
                if(rel > max) max = rel;
            }
            if(orientation == DirectionalConstants.HORIZONTAL) {
                return new Size(sum, max);
            } else {
                return new Size(max, sum);
            }
        }

        public virtual Component GetOwner() {
            return owner;
        }

        public virtual Component[] GetComponentsLeftTo(Component c) {
            if(orientation == DirectionalConstants.VERTICAL) return new Component[0];
            int ind = comps.IndexOf(c);
            List<Component> list = new List<Component>();
            for(int i = ind - 1; i >= 0; i--) {
                list.Add(comps[i]);
            }
            return list.ToArray();
        }

        public virtual Component[] GetComponentsRightTo(Component c) {
            if(orientation == DirectionalConstants.VERTICAL) return new Component[0];
            int ind = comps.IndexOf(c);
            List<Component> list = new List<Component>();
            for(int i = ind + 1; i < comps.Count; i++) {
                list.Add(comps[i]);
            }
            return list.ToArray();
        }

        public virtual Component[] GetComponentsAbove(Component c) {
            if(orientation == DirectionalConstants.HORIZONTAL) return new Component[0];
            int ind = comps.IndexOf(c);
            List<Component> list = new List<Component>();
            for(int i = ind - 1; i >= 0; i--) {
                list.Add(comps[i]);
            }
            return list.ToArray();
        }

        public virtual Component[] GetComponentsBelow(Component c) {
            if(orientation == DirectionalConstants.HORIZONTAL) return new Component[0];
            int ind = comps.IndexOf(c);
            List<Component> list = new List<Component>();
            for(int i = ind + 1; i < comps.Count; i++) {
                list.Add(comps[i]);
            }
            return list.ToArray();
        }

        public virtual Component[] OrderedComponentsLTR() {
            return comps.ToArray();
        }

        public virtual Component[] OrderedComponentsTTB() {
            return comps.ToArray();
        }

    }

}

