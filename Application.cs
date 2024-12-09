using System.Drawing;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Tutorial
{
    class Application
    {
        private static Application? _instance;

        private GL? _gl;

        private IWindow _window;

        private Application(string title, int width, int height)
        {
            //Create a window.
            var options = WindowOptions.Default with
            {
                Title = title,
                Size = new Vector2D<int>(width, height),
            };

            _window = Window.Create(options);

            //Assign events.
            _window.Load += OnLoad;
            _window.Update += OnUpdate;
            _window.Render += OnRender;
            _window.FramebufferResize += OnFramebufferResize;
        }

        private void OnLoad()
        {
            //Set-up input context.
            var input = _window.CreateInput();
            for (int i = 0; i < input?.Keyboards.Count; i++) input.Keyboards[i].KeyDown += OnKeyDown;

            _gl = _window.CreateOpenGL();
            _gl.ClearColor(Color.Black);
        }

        private void OnUpdate(double delta) { }

        private void OnRender(double delta)
        {
            _gl?.Clear(ClearBufferMask.ColorBufferBit);
        }

        private void OnFramebufferResize(Vector2D<int> newSize) { }

        private void OnKeyDown(IKeyboard keyboard, Key key, int keyCode)
        {
            //Check to close the window on escape.
            if (key == Key.Escape) _window.Close();
        }

        public void Run()
        {
            //Run the window.
            _window.Run();

            // window.Run() is a BLOCKING method - this means that it will halt execution of any code in the current
            // method until the window has finished running. Therefore, this dispose method will not be called until you
            // close the window.
            _window.Dispose();
        }

        public static Application Create(string title, int width, int height)
        {
            if (_instance is null) _instance = new Application(title, width, height);

            return _instance;
        }
    }
}
