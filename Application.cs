using System.Drawing;
using Silk.NET.Core.Loader;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Tutorial
{
    class Application
    {
        private static Application? _instance;

        private Texture? _texture;
        private VertexArray? _vertexArray;
        private Shader? _shader;

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
            _gl.ClearColor(Color.White);
            _gl.Enable(EnableCap.Blend);
            _gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            //Create vertex array
            _vertexArray = new VertexArray(_gl);

            //Create vertex buffer
            var vertices = new float[]
            {
                // positions        // colors          // Tex Coord
                -0.5f,  0.5f, 0.0f, 0.6f,  0.4f,  0.8f,  0.0f, 1.0f,
                 0.5f,  0.5f, 0.0f, 0.85f, 0.44f, 0.84f, 1.0f, 1.0f,
                -0.5f, -0.5f, 0.0f, 0.7f,  0.7f,  0.9f,  0.0f, 0.0f,
                 0.5f, -0.5f, 0.0f, 0.88f, 0.69f, 0.87f, 1.0f, 0.0f,
            };
            var vertexBuffer = new BufferObject<float>(_gl, BufferTargetARB.ArrayBuffer, vertices);

            //Create index buffer
            var indices = new uint[]
            {
                0u, 1u, 2u,
                2u, 1u, 3u,
            };
            var elementBuffer = new BufferObject<uint>(_gl, BufferTargetARB.ElementArrayBuffer, indices);

            _shader = Shader.LoadFromFile(_gl, "./vert.glsl", "./frag.glsl");
            _shader.SetUniform("uTexture");

            _texture = new Texture(_gl, "silk.png");

            //Set vertex attribute pointers
            const uint posLoc = 0;
            _vertexArray.VertexAttributePointer<float>(posLoc, 3, VertexAttribPointerType.Float, 8, 0);

            const uint colorLoc = 1;
            _vertexArray.VertexAttributePointer<float>(colorLoc, 3, VertexAttribPointerType.Float, 8, 3);

            const uint texCoordLoc = 2;
            _vertexArray.VertexAttributePointer<float>(texCoordLoc, 2, VertexAttribPointerType.Float, 8, 6);
        }

        private void OnUpdate(double delta) { }

        private unsafe void OnRender(double delta)
        {
            _gl?.Clear(ClearBufferMask.ColorBufferBit);

            //Render the triangle
            _vertexArray?.Bind();
            _shader?.Use();
            _texture?.Bind();
            _gl?.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, (void*)0);
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
