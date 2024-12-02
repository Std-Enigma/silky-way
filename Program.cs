using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace Tutorial
{
    class Program
    {
        private static IWindow? _window;

        private static void Main(string[] args)
        {
            //Create a window.
            var options = WindowOptions.Default with
            {
                Size = new Vector2D<int>(1280, 720),
                Title = "LearnOpenGL with Silk.NET"
            };

            _window = Window.Create(options);

            //Assign events.
            _window.Load += OnLoad;
            _window.Update += OnUpdate;
            _window.Render += OnRender;
            _window.FramebufferResize += OnFramebufferResize;

            //Run the window.
            _window.Run();

            // window.Run() is a BLOCKING method - this means that it will halt execution of any code in the current
            // method until the window has finished running. Therefore, this dispose method will not be called until you
            // close the window.
            _window.Dispose();
        }


        private static void OnLoad()
        {
            //Set-up input context.
            var input = _window?.CreateInput();
            for (int i = 0; i < input?.Keyboards.Count; i++) input.Keyboards[i].KeyDown += KeyDown;
        }

        private static void OnRender(double delta) { }

        private static void OnUpdate(double delta) { }

        private static void OnFramebufferResize(Vector2D<int> newSize) { }

        private static void KeyDown(IKeyboard keyboard, Key key, int keyCode)
        {
            //Check to close the window on escape.
            if (key == Key.Escape) _window?.Close();
        }
    }
}
