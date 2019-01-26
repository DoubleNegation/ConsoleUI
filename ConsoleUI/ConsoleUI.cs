using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace ConsoleUI {

    public class Window : Component {

        private static bool initialized = false;
        private static Window instance;
        private static BorderLayout windowLayout = new BorderLayout();

        public static Window Init() {
            if(initialized) throw new Exception("Already Initialized");
            initialized = true;
            return new Window();
        }

        private static void Loop() {
            while(true) {
                instance.Update();
                Thread.Sleep(100);
            }
        }

        private static void InputReader() {
            while(true) {
                ConsoleKeyInfo c = Console.ReadKey(true);
                if((c.Modifiers & ConsoleModifiers.Control) != 0 && c.Key.ToString() == "C") {
                    Console.CursorVisible = true;
                    Console.ResetColor();
                    Console.Clear();
                    Console.WriteLine("CONTROL-C  --  EXITING NOW");
                    Environment.Exit(0);
                }
                //if nothing is focused, there's nothing to send the event to
                if(instance.focusMgmnt.FocusedComponent == null) continue;
                //send KeyEvent
                instance.Invoke(() => instance.EnqueueEvent(new KeyEvent(instance.focusMgmnt.FocusedComponent, c)));
            }
        }

        private BorderLayout layout;
        private Thread updateLoop;
        private Thread inputLoop;
        private ConsoleUtils utils;
        private ConsoleBuffer drawingBuffer;
        private ConsoleBuffer currentConsole;
        private EventQueue queue;
        private Graphics graphics;
        private FocusMove focusMgmnt;

        private Window() : base(windowLayout) {
            instance = this;
            layout = windowLayout;
            Color bg = Background;
            Console.BackgroundColor = bg == null ? Color.BLACK.ConsoleColorValue : bg.ConsoleColorValue;
            Console.Clear();
            Console.TreatControlCAsInput = true;
            Console.CursorVisible = false;
            utils = new ConsoleUtils();
            drawingBuffer = new ConsoleBuffer(Console.WindowWidth, Console.WindowHeight, bg == null ? Color.BLACK : bg, Color.WHITE);
            currentConsole = new ConsoleBuffer(Console.WindowWidth, Console.WindowHeight, bg == null ? Color.BLACK : bg, Color.WHITE);
            graphics = new Graphics(drawingBuffer, 0, 0, Console.WindowWidth, Console.WindowHeight);
            focusMgmnt = new FocusMove(this);
            queue = new EventQueue();
            updateLoop = new Thread(new ThreadStart(Loop));
            updateLoop.Start();
            inputLoop = new Thread(new ThreadStart(InputReader));
            inputLoop.Start();
            Validate();
            //This Listener receives KeyEvents and converts them into ActionEvents and FocusMove calls if
            //they haven't been consumed yet.
            AddKeyListener(new KeyAdapter().KeyTyped(e => {
                //if nothing is focused, there's nothing to send the event to
                if(focusMgmnt.FocusedComponent == null) return;
                ConsoleKeyInfo c = e.Key;
                //handle inputs
                if(c.Key == ConsoleKey.UpArrow) {
                    instance.focusMgmnt.Up();
                } else if(c.Key == ConsoleKey.DownArrow) {
                    instance.focusMgmnt.Down();
                } else if(c.Key == ConsoleKey.LeftArrow) {
                    instance.focusMgmnt.Left();
                } else if(c.Key == ConsoleKey.RightArrow) {
                    instance.focusMgmnt.Right();
                } else if(c.Key == ConsoleKey.Enter || c.Key == ConsoleKey.Spacebar) {
                    EnqueueEvent(new ActionEvent(focusMgmnt.FocusedComponent));
                }
            }));
        }

        public void ConsoleWindowInvalidated() {
            MakeSureToRunOnQueueThread(_ConsoleWindowInvalidated);
        }

        private void _ConsoleWindowInvalidated() {
            Color bg = Background;
            drawingBuffer = new ConsoleBuffer(Console.WindowWidth, Console.WindowHeight, bg == null ? Color.BLACK : bg, Color.WHITE);
            currentConsole = new ConsoleBuffer(Console.WindowWidth, Console.WindowHeight, bg == null ? Color.BLACK : bg, Color.WHITE);
            graphics = new Graphics(drawingBuffer, 0, 0, Console.WindowWidth, Console.WindowHeight);
            Console.BackgroundColor = bg == null ? Color.BLACK.ConsoleColorValue : bg.ConsoleColorValue;
            try {
                Console.Clear();
            } catch(System.IO.IOException) {
                // happens when the window height becomes 0
            }
            Console.CursorVisible = false;
            Invalidate();
            Validate();
            _Paint();
        }

        private void Update() {
            utils.Update(this);
            queue.CallAllActions();
        }

        public void Paint() {
            MakeSureToRunOnQueueThread(_Paint);
        }

        private void _Paint() {
            try {
                //clear buffer to prevent things remaining there which shouldn't
                graphics.Color = Background == null ? Color.BLACK : Background;
                graphics.FillBackgroundRect(0, 0, drawingBuffer.Width, drawingBuffer.Height);
                graphics.FillRect(' ', 0, 0, drawingBuffer.Width, drawingBuffer.Height);
                //paint everything onto the buffer
                PaintComponents(graphics);
                //draw differences between currentConsole and drawingBuffer to the console
                for (int y = 0; y < Console.WindowHeight; y++) {
                    for (int x = 0; x < Console.WindowWidth; x++) {
                        //safety measure to prevent crashes when user is violently resizing the window
                        if (!(x < drawingBuffer.Width && y < drawingBuffer.Height)) continue;
                        if (currentConsole[x, y] != drawingBuffer[x, y] ||
                                currentConsole.GetForegroundAt(x, y) != drawingBuffer.GetForegroundAt(x, y) ||
                                currentConsole.GetBackgroundAt(x, y) != drawingBuffer.GetBackgroundAt(x, y)) {
                            Console.SetCursorPosition(x, y);
                            Console.ForegroundColor = drawingBuffer.GetForegroundAt(x, y).ConsoleColorValue;
                            Console.BackgroundColor = drawingBuffer.GetBackgroundAt(x, y).ConsoleColorValue;
                            Console.Write(drawingBuffer[x, y]);
                        }
                        Thread.Sleep(0);
                    }
                }
                //update currentConsole
                drawingBuffer.copyTo(currentConsole);
                //make cursor less irritating
                Console.SetCursorPosition(0, 0);
            } finally{}/*catch(Exception e) {
                if(!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows)) {
                    throw e;
                }
            }*/
        }

        public void Invoke(ThreadStart method) {
            queue.Add(method);
        }

        public void InvokeLater(ThreadStart method) {
            queue.PQQAdd(method);
        }

        public void PaintLater() {
            queue.PQQAdd(_Paint);
        }

        private void MakeSureToRunOnQueueThread(ThreadStart method) {
            if(Thread.CurrentThread != updateLoop) {
                queue.Add(method);
            } else {
                method();
            }
        }

        public void EnqueueEvent(Event e) {
            new EnqueuedEvent(queue, e);
        }

        internal class EnqueuedEvent {
            private EventQueue q;
            private Event e;
            private Component c;
            internal EnqueuedEvent(EventQueue q, Event e) {
                this.q = q;
                this.e = e;
                this.c = e.Source;
                q.Add(Do);
            }
            internal void Do() {
                c.DispatchEvent(e);
                if(!e.Consumed) {
                    if(c == c.GetWindow()) return;
                    c = c.Parent;
                    q.Add(Do);
                }
            }
        }

        public void SetFocusedComponent(Component c) {
            focusMgmnt.FocusedComponent = c;
        }

        /* OVERRIDE COMPONENT METHODS THAT REFER TO THE PARENT (which doesn't exist here) */

        public override ILayoutManager Layout {
            get {
                return layout;
            }
        }

        public override Window GetWindow() {
            return this;
        }

        public override Size GetSize() {
            return new Size(Console.WindowWidth, Console.WindowHeight);
        }

        public override int GetWidth() {
            return Console.WindowWidth;
        }

        public override int GetHeight() {
            return Console.WindowHeight;
        }

        public override Point GetLocationInWindow() {
            return new Point(0, 0);
        }

    }

}

