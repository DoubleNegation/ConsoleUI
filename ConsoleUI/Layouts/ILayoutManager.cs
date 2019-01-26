using System;
using System.Drawing;

namespace ConsoleUI {

    public interface ILayoutManager {
        void Invalidate();
        void Validate();
        Component[] GetComponents();
        void Add(Component c);
        void Add(Component c, Object constraints);
        void Remove(Component c);
        void RemoveAll();
        Size GetSizeOf(Component c);
        Point GetLocationOf(Component c);
        Window GetWindow();
        void OnAddedToComponent(Component c);
        Size GetPreferredSize();
        Component GetOwner();
        Component[] GetComponentsLeftTo(Component c);
        Component[] GetComponentsRightTo(Component c);
        Component[] GetComponentsAbove(Component c);
        Component[] GetComponentsBelow(Component c);
        Component[] OrderedComponentsLTR();
        Component[] OrderedComponentsTTB();
    }

}

