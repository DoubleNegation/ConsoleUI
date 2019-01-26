using System;
using System.Collections.Generic;
using System.Drawing;

namespace ConsoleUI {

    public class BorderLayout : ILayoutManager {

        private Component owner = null;
        private bool valid;

        private Component top = null;
        private Component right = null;
        private Component bottom = null;
        private Component left = null;
        private Component center = null;

        private int topHeight = 0;
        private int bottomHeight = 0;
        private int leftWidth = 0;
        private int rightWidth = 0;

        public bool Valid {
            get {
                return valid;
            }
        }

        public virtual void Invalidate() {
            valid = false;
            if(top != null) top.Invalidate();
            if(right != null) right.Invalidate();
            if(bottom != null) bottom.Invalidate();
            if(left != null) left.Invalidate();
            if(center != null) center.Invalidate();
        }

        public virtual void Validate() {
            Size availableSpace = owner.GetSize();
            int aw = availableSpace.Width;
            int ah = availableSpace.Height;
            int l = left == null ? 0 : left.GetPreferredSize().Width;
            int r = right == null ? 0 : right.GetPreferredSize().Width;
            int t = top == null ? 0 : top.GetPreferredSize().Height;
            int b = bottom == null ? 0 :bottom.GetPreferredSize().Height;
            if(l + r > aw) {
                if(l > aw/2 && r > aw/2) {
                    if(aw % 2 == 0) {
                        leftWidth = aw / 2;
                        rightWidth = aw / 2;
                    } else if(r > l) {
                        leftWidth = aw / 2;
                        rightWidth = aw / 2 + 1;
                    } else {
                        leftWidth = aw / 2 + 1;
                        rightWidth = aw / 2;
                    }
                } else if(r > aw / 2) {
                    leftWidth = l;
                    rightWidth = aw - l;
                } else {
                    leftWidth = aw - r;
                    rightWidth = r;
                }
            } else {
                leftWidth = l;
                rightWidth = r;
            }
            if(t + b > ah) {
                if(t > ah/2 && b > ah/2) {
                    if(ah % 2 == 0) {
                        topHeight = ah / 2;
                        bottomHeight = ah / 2;
                    } else if(b > t) {
                        topHeight = ah / 2;
                        bottomHeight = ah / 2 + 1;
                    } else {
                        topHeight = ah / 2 + 1;
                        bottomHeight = ah / 2;
                    }
                } else if(b > ah / 2) {
                    topHeight = t;
                    bottomHeight = ah - t;
                } else {
                    topHeight = ah - b;
                    bottomHeight = b;
                }
            } else {
                topHeight = t;
                bottomHeight = b;
            }
            valid = true;
            if(top != null) top.Validate();
            if(right != null) right.Validate();
            if(bottom != null) bottom.Validate();
            if(left != null) left.Validate();
            if(center != null) center.Validate();
        }

        public virtual Component[] GetComponents() {
            int nonnulls = 0;
            if(top != null) nonnulls++;
            if(right != null) nonnulls++;
            if(bottom != null) nonnulls++;
            if(left != null) nonnulls++;
            if(center != null) nonnulls++;
            Component[] ret = new Component[nonnulls];
            int i = 0;
            if(top != null) ret[i++] = top;
            if(right != null) ret[i++] = right;
            if(bottom != null) ret[i++] = bottom;
            if(left != null) ret[i++] = left;
            if(center != null) ret[i++] = center;
            return ret;
        }

        public virtual void Add(Component c) {
            Add(c, DirectionalConstants.CENTER);
        }

        public virtual void Add(Component c, Object constraints) {
            if(!(constraints is DirectionalConstants)) center = c;
            else if(((DirectionalConstants)constraints) == DirectionalConstants.RIGHT) right = c;
            else if(((DirectionalConstants)constraints) == DirectionalConstants.LEFT) left = c;
            else if(((DirectionalConstants)constraints) == DirectionalConstants.BOTTOM) bottom = c;
            else if(((DirectionalConstants)constraints) == DirectionalConstants.TOP) top = c;
            else center = c;
            Invalidate();
            c.ParentLayout = this;
        }

        public virtual void Remove(Component c) {
            if(top == c) top = null;
            if(right == c) right = null;
            if(bottom == c) bottom = null;
            if(left == c) left = null;
            if(center == c) center = null;
            Invalidate();
        }

        public virtual void RemoveAll() {
            top = right = bottom = left = center = null;
            Invalidate();
        }

        public virtual Size GetSizeOf(Component c) {
            if(c == top) {
                return new Size(owner.GetWidth(), topHeight);
            } else if(c == bottom) {
                return new Size(owner.GetWidth(), bottomHeight);
            } else if(c == left) {
                return new Size(leftWidth, owner.GetHeight() - topHeight - bottomHeight);
            } else if(c == right) {
                return new Size(rightWidth, owner.GetHeight() - topHeight - bottomHeight);
            } else if(c == center) {
                Size full = owner.GetSize();
                return new Size(full.Width - leftWidth - rightWidth, full.Height - topHeight - bottomHeight);
            }
            return new Size(0, 0);
        }

        public virtual Point GetLocationOf(Component c) {
            if(c == top) {
                return new Point(0, 0);
            } else if(c == bottom) {
                return new Point(0, owner.GetHeight() - bottomHeight);
            } else if(c == left) {
                return new Point(0, topHeight);
            } else if(c == right) {
                return new Point(owner.GetWidth() - rightWidth, topHeight);
            } else if(c == center) {
                return new Point(leftWidth, topHeight);
            }
            return new Point(0, 0);
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
            Size centerP = center == null ? new Size(0, 0) : center.GetPreferredSize();
            Size leftP = left == null ? new Size(0, 0) : left.GetPreferredSize();
            Size rightP = right == null ? new Size(0, 0) : right.GetPreferredSize();
            Size topP = top == null ? new Size(0, 0) : top.GetPreferredSize();
            Size bottomP = bottom == null ? new Size(0, 0) : bottom.GetPreferredSize();
            return new Size(Math.Max(Math.Max(topP.Width, bottomP.Width), centerP.Width + leftP.Width + rightP.Width),
                    Math.Max(Math.Max(leftP.Height, rightP.Height), centerP.Height) + topP.Height + bottomP.Height);
        }

        public virtual Component GetOwner() {
            return owner;
        }

        public virtual Component[] GetComponentsLeftTo(Component c) {
            List<Component> list = new List<Component>();
            if(c == right && center != null) {
                list.Add(center);
            }
            if((c == center || c == right) && left != null) {
                list.Add(left);
            }
            return list.ToArray();
        }

        public virtual Component[] GetComponentsRightTo(Component c) {
            List<Component> list = new List<Component>();
            if((c == center || c == left) && right != null) {
                list.Add(right);
            }
            if(c == left && center != null) {
                list.Add(center);
            }
            return list.ToArray();
        }

        public virtual Component[] GetComponentsAbove(Component c) {
            List<Component> list = new List<Component>();
            if(c == bottom && right != null) list.Add(right);
            if(c == bottom && center != null) list.Add(center);
            if(c == bottom && left != null) list.Add(left);
            if(c != top && top != null) list.Add(top);
            return list.ToArray();
        }

        public virtual Component[] GetComponentsBelow(Component c) {
            List<Component> list = new List<Component>();
            if(c == top && left != null) list.Add(left);
            if(c == top && center != null) list.Add(center);
            if(c == top && right != null) list.Add(right);
            if(c != bottom && bottom != null) list.Add(bottom);
            return list.ToArray();
        }

        public virtual Component[] OrderedComponentsLTR() {
            List<Component> list = new List<Component>();
            if(left != null) list.Add(left);
            if(top != null) list.Add(top);
            if(center != null) list.Add(center);
            if(bottom != null) list.Add(bottom);
            if(right != null) list.Add(right);
            return list.ToArray();
        }

        public virtual Component[] OrderedComponentsTTB() {
            List<Component> list = new List<Component>();
            if(top != null) list.Add(top);
            if(left != null) list.Add(left);
            if(center != null) list.Add(center);
            if(right != null) list.Add(right);
            if(bottom != null) list.Add(bottom);
            return list.ToArray();
        }

    }

}

