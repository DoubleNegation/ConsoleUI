using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ConsoleUI {

    public class GridLayout : ILayoutManager {

        private int rows;
        private int cols;
        private int hgap;
        private int vgap;
        private List<Component> comps = new List<Component>();
        private int layedOutWidth = 0;
        private int layedOutHeight = 0;
        private int numExtraWidth = 0;
        private int numExtraHeight = 0;
        private Component owner = null;

        public GridLayout(int rows, int cols) {
            if(rows < 0 || cols < 0) throw new ArgumentException("Argument out of range. Row and column numbers must be positive.");
            if(rows == 0 && cols == 0) throw new ArgumentException("Rows and columns may not both be zero.");
            this.rows = rows;
            this.cols = cols;
            hgap = 0;
            vgap = 0;
        }

        public GridLayout(int rows, int cols, int hgap, int vgap) {
            if(rows < 0 || cols < 0 || hgap < 0 || vgap < 0) throw new ArgumentException("Argument out of range. Row and column numbers and gap sizes must be positive.");
            if(rows == 0 && cols == 0) throw new ArgumentException("Rows and columns may not both be zero.");
            this.rows = rows;
            this.cols = cols;
            this.hgap = hgap;
            this.vgap = vgap;
        }

        public virtual void Invalidate() {
            foreach(Component c in comps) {
                if(c == null) continue;
                c.Invalidate();
            }
        }

        public virtual void Validate() {
            Size availableSpace = owner.GetSize();
            int rows = this.rows;
            int cols = this.cols;
            if(cols == 0) {
                cols = (int)Math.Ceiling(comps.Count / (decimal)rows);
            } else if(rows == 0) {
                rows = (int)Math.Ceiling(comps.Count / (decimal)cols);
            }
            //calculate cell size
            layedOutWidth = availableSpace.Width / cols;
            layedOutHeight = availableSpace.Height / rows;
            numExtraWidth = availableSpace.Width - (layedOutWidth * cols);
            numExtraHeight = availableSpace.Height - (layedOutHeight * rows);
            //account for gaps
            layedOutWidth -= hgap;
            numExtraWidth += hgap;
            while(numExtraWidth >= cols) {
                layedOutWidth++;
                numExtraWidth -= cols;
            }
            layedOutHeight -= vgap;
            numExtraHeight += vgap;
            while(numExtraHeight >= rows) {
                layedOutHeight++;
                numExtraHeight -= rows;
            }
            //layout is calulated, validate components inside
            foreach(Component c in comps) {
                if(c == null) continue;
                c.Validate();
            }
        }

        public virtual Component[] GetComponents() {
            return (from comp in comps where comp != null select comp).ToArray();
        }

        public virtual void Add(Component c) {
            int found = -1;
            for(int i = 0; i < comps.Count; i++) {
                if(comps[i] == null) {
                    found = i;
                    break;
                }
            }
            if(found == -1) {
                if((rows * cols) != 0 && comps.Count == (rows * cols)) {
                    throw new ArgumentException("The grid is already full. Can not add any more components.");
                }
                comps.Add(c);
            } else {
                comps.Insert(found, c);
            }
            Invalidate();
            c.ParentLayout = this;
        }

        public virtual void Add(Component c, Object constraints) {
            if(constraints is Point) {
                Point p = (Point) constraints;
                if(cols != 0 && p.X >= cols) throw new ArgumentException("Column out of bounds");
                if(rows != 0 && p.Y >= rows) throw new ArgumentException("Row out of bounds");
                int index = cols == 0 ? p.X * rows + p.Y : p.Y * cols + p.X;
                while(comps.Count < index + 1) {
                    comps.Add(null);
                }
                comps.RemoveAt(index);
                comps.Insert(index, c);
                Invalidate();
                c.ParentLayout = this;
            } else {
                Add(c);
            }
        }

        public virtual void Remove(Component c) {
            comps.Insert(comps.IndexOf(c), null);
            Invalidate();
        }

        public virtual void RemoveAll() {
            comps.Clear();
            Invalidate();
        }

        public virtual Point GetGridLocationOf(Component c) {
            int ind = comps.IndexOf(c);
            int x = cols == 0 ? ind / rows : ind % cols;
            int y = cols == 0 ? ind % rows : ind / cols;
            return new Point(x, y);
        }

        public virtual Size GetSizeOf(Component c) {
            Point cell = GetGridLocationOf(c);
            return new Size(cell.X < numExtraWidth ? layedOutWidth + 1 : layedOutWidth,
                            cell.Y < numExtraHeight ? layedOutHeight + 1 : layedOutHeight);
        }

        public virtual Point GetLocationOf(Component c) {
            Point cell = GetGridLocationOf(c);
            return new Point(cell.X * (layedOutWidth + hgap) + Math.Min(numExtraWidth, cell.X),
                            cell.Y * (layedOutHeight + vgap) + Math.Min(numExtraHeight, cell.Y));
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
            int w = 0;
            int h = 0;
            foreach(Component comp in comps) {
                if(comp != null) {
                    Size s = comp.GetPreferredSize();
                    w = Math.Max(w, s.Width);
                    h = Math.Max(h, s.Height);
                }
            }
            return new Size(w, h);
        }

        public virtual Component GetOwner() {
            return owner;
        }

        public virtual List<Component> GetRow(int index) {
            List<Component> list = new List<Component>();
            if(cols == 0) {
                for(int i = index; i < comps.Count; i += rows) {
                    Component comp = comps[i];
                    if(comp != null) {
                        list.Add(comp);
                    }
                }
            } else {
                for(int i = 0; i < cols; i++) {
                    Component comp = comps[index * cols + i];
                    if(comp != null) {
                        list.Add(comp);
                    }
                }
            }
            return list;
        }

        public virtual List<Component> GetColumn(int index) {
            List<Component> list = new List<Component>();
            if(cols == 0) {
                for(int i = 0; i < rows; i++) {
                    Component comp = comps[index * rows + i];
                    if(comp != null) {
                        list.Add(comp);
                    }
                }
            } else {
                for(int i = index; i < comps.Count; i += cols) {
                    Component comp = comps[i];
                    if(comp != null) {
                        list.Add(comp);
                    }
                }
            }
            return list;
        }

        public virtual Component[] GetComponentsLeftTo(Component c) {
            Point cell = GetGridLocationOf(c);
            List<Component> list = new List<Component>();
            for(int i = cell.X - 1; i >= 0; i--) {
                list.AddRange(GetColumn(i));
            }
            OrderByDistanceFrom(c, list);
            return list.ToArray();
        }

        public virtual Component[] GetComponentsRightTo(Component c) {
            Point cell = GetGridLocationOf(c);
            List<Component> list = new List<Component>();
            for(int i = cell.X + 1; i < (cols == 0 ? comps.Count / rows : cols); i++) {
                list.AddRange(GetColumn(i));
            }
            OrderByDistanceFrom(c, list);
            return list.ToArray();
        }

        public virtual Component[] GetComponentsAbove(Component c) {
            Point cell = GetGridLocationOf(c);
            List<Component> list = new List<Component>();
            for(int i = cell.Y - 1; i >= 0; i--) {
                list.AddRange(GetRow(i));
            }
            OrderByDistanceFrom(c, list);
            return list.ToArray();
        }

        public virtual Component[] GetComponentsBelow(Component c) {
            Point cell = GetGridLocationOf(c);
            List<Component> list = new List<Component>();
            for(int i = cell.Y + 1; i < (rows == 0 ? comps.Count / cols : rows); i++) {
                list.AddRange(GetRow(i));
            }
            OrderByDistanceFrom(c, list);
            return list.ToArray();
        }

        public virtual Component[] OrderedComponentsLTR() {
            List<Component> list = new List<Component>();
            for(int i = 0; i < (cols == 0 ? comps.Count / rows : cols); i++) {
                list.AddRange(GetColumn(i));
            }
            return list.ToArray();
        }

        public virtual Component[] OrderedComponentsTTB() {
            List<Component> list = new List<Component>();
            for(int i = 0; i < (rows == 0 ? comps.Count / cols : rows); i++) {
                list.AddRange(GetRow(i));
            }
            return list.ToArray();
        }

        protected virtual void OrderByDistanceFrom(Component origin, List<Component> elements) {
            SortedDictionary<int, List<Component>> dict = new SortedDictionary<int, List<Component>>();
            Point o = GetLocationOf(origin);
            foreach(Component comp in elements) {
                Point loc = GetLocationOf(comp);
                int x = Math.Abs(loc.X - o.X) * 100;
                int y = Math.Abs(loc.Y - o.Y) * 100;
                int hypothenuse = (int)Math.Sqrt(x * x + y * y);
                if(!dict.Keys.Contains(hypothenuse)) {
                    dict.Add(hypothenuse, new List<Component>());
                }
                dict[hypothenuse].Add(comp);
            }
            elements.Clear();
            foreach(KeyValuePair<int, List<Component>> kvp in dict) {
                elements.AddRange(kvp.Value);
            }
        }

    }

}

