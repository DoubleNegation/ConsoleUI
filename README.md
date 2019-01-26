# C# .NET  ConsoleUI
ConsoleUI is a C# .NET library designed for .NET Core 2.1 which implements a simple widget toolkit for creating graphical user interfaces in the console.  
  
__DISCLAIMER: I do not plan on actively maintaining this library. It was written for the sole purpose of having something impressive for a school assignment and was only published *in case someone finds it useful*. Use of this library is therefore discouraged.__

## Usage Instructions
To use ConsoleUI, it is important to know about two concepts which it uses:
 - __Component__: A component is a widget which is displayed on screen. It is responsible for drawing itself into its space.
 - __Layout Manager__: A layout manager is responsible for laying out components inside of other components. Every component has to have a layout manager attached to it, which lays out its children. The number of components which a layout manager can hold depends on the implementation.
Your code could then look like this:
```c#
//Initialize the console window for use with ConsoleUI and create the root component of the component tree.
//The layout manager of the window is a BorderLayout by default. Please do not change that directly, as it might break stuff.
Window win = Window.Init();
//We want this little program to use a StackLayout on the root component.
//Because we can not change the layout of the Window, we create another component with a StackLayout and add it to the Window.
Component root = new Component(new StackLayout());
//The add method of component is a pass-through to the add method of the layout manager.
win.Add(root, DirectionalConstants.CENTER);
//The topmost component in out program should be a label which displays a message.
Label label = new Label("Hello World");
root.Add(label);
//We also want to add a button which changes the label's text and design
Button b1 = new Button("Press Me!");
b1.AddActionListener(new ActionAdapter().ActionPerformed(ev => {
    label.Text = "You pressed it :O";
    label.Foreground = Color.PINK;
    //Make sure everything is redrawn
    win.Paint();
}));
root.Add(b1);
//One component which can be interacted with needs to be set the focused component initially,
//otherwise the user won't be able to interact with the application.
win.SetFocusedComponent(b1);
//Finally, we want to add a button which resets the label to it's initial state.
Button b2 = new Button("Reset");
b2.AddActionListener(new ActionAdapter().ActionPerformed(ev => {
	label.Text = "Hello World";
    label.Foreground = null;
    //Make sure everything is redrawn
    win.Paint();
}));
root.Add(b2);
//Before anything is displayed to the user, the layouts need to be calculated
win.Revalidate();
//Finally, draw everything to the console
win.Paint();
```
For any further help, please refer to the source code of the library.

