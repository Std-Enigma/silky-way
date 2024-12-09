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

        private uint _vao;
        private uint _program;

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

        private unsafe void OnLoad()
        {
            //Set-up input context.
            var input = _window.CreateInput();
            for (int i = 0; i < input?.Keyboards.Count; i++) input.Keyboards[i].KeyDown += OnKeyDown;

            _gl = _window.CreateOpenGL();
            _gl.ClearColor(Color.Black);

            //Create vertex array
            _vao = _gl.GenVertexArray();
            _gl.BindVertexArray(_vao);

            //Create vertex buffer
            var vertices = new float[]
            {
                -0.5f, -0.5f, 0.0f,
                 0.0f,  0.5f, 0.0f,
                 0.5f, -0.5f, 0.0f,
            };
            var vbo = _gl.GenBuffer();
            _gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
            fixed (float* buf = vertices) _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);

            //Create vertex shader
            var vertCode = File.ReadAllText("./vert.glsl");
            var vertShader = _gl.CreateShader(ShaderType.VertexShader);
            _gl.ShaderSource(vertShader, vertCode);
            _gl.CompileShader(vertShader);
            _gl.GetShader(vertShader, ShaderParameterName.CompileStatus, out int vStatus);
            if (vStatus != (int)GLEnum.True) throw new Exception($"Vertex shader failed to compile: {_gl.GetShaderInfoLog(vertShader)}");

            //Create fragment shader
            var fragCode = File.ReadAllText("./frag.glsl");
            var fragShader = _gl.CreateShader(ShaderType.FragmentShader);
            _gl.ShaderSource(fragShader, fragCode);
            _gl.CompileShader(fragShader);
            _gl.GetShader(fragShader, ShaderParameterName.CompileStatus, out int fStatus);
            if (vStatus != (int)GLEnum.True) throw new Exception($"Fragment shader failed to compile: {_gl.GetShaderInfoLog(fragShader)}");

            //Create shader program
            _program = _gl.CreateProgram();
            _gl.AttachShader(_program, vertShader);
            _gl.AttachShader(_program, fragShader);
            _gl.LinkProgram(_program);
            _gl.GetProgram(_program, ProgramPropertyARB.LinkStatus, out int lStatus);
            if (lStatus != (int)GLEnum.True) throw new Exception($"Shader program failed to link: {_gl.GetProgramInfoLog(_program)}");

            //Delete shader resources
            _gl.DetachShader(_program, vertShader);
            _gl.DetachShader(_program, fragShader);
            _gl.DeleteShader(vertShader);
            _gl.DeleteShader(fragShader);

            //Set vertex attribute pointers
            const uint posLoc = 0;
            _gl.EnableVertexAttribArray(posLoc);
            _gl.VertexAttribPointer(posLoc, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (void*)0);

            //Unbind resources
            _gl.BindVertexArray(0);
            _gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        }

        private void OnUpdate(double delta) { }

        private void OnRender(double delta)
        {
            _gl?.Clear(ClearBufferMask.ColorBufferBit);

            //Render the triangle
            _gl?.BindVertexArray(_vao);
            _gl?.UseProgram(_program);
            _gl?.DrawArrays(PrimitiveType.Triangles, 0, 3);
        }

        private void OnFramebufferResize(Vector2D<int> newSize)
        {
            // Resize viewport the correct size
            _gl?.Viewport(newSize);
        }

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
