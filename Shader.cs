using Silk.NET.OpenGL;

namespace Tutorial
{
    class Shader : IDisposable
    {
        private GL _gl;
        private uint _handle;
        private bool _disposedValue;

        public Shader(GL gl, string vertSource, string fragSource)
        {
            _gl = gl;
            var vertShader = CreateShader(ShaderType.VertexShader, vertSource);
            var fragShader = CreateShader(ShaderType.FragmentShader, fragSource);

            // Create the shader program
            _handle = _gl.CreateProgram();
            gl.AttachShader(_handle, vertShader);
            gl.AttachShader(_handle, fragShader);
            gl.LinkProgram(_handle);
            gl.GetProgram(_handle, ProgramPropertyARB.LinkStatus, out int lStatus);
            if (lStatus != (int)GLEnum.True)
            {
                var log = gl.GetProgramInfoLog(_handle);
                throw new Exception($"Shader linking failed: {log}");
            }

            // Remove the unused resources
            gl.DetachShader(_handle, vertShader);
            gl.DetachShader(_handle, fragShader);
            gl.DeleteShader(vertShader);
            gl.DeleteShader(fragShader);
        }

        public void Use()
        {
            // Use the current shader program
            _gl.UseProgram(_handle);
        }

        public void SetUniform(string name)
        {
            // Set the uniform of this program
            var location = _gl.GetUniformLocation(_handle, name);
            _gl.Uniform1(location, 0);
        }

        public static Shader LoadFromFile(GL gl, string vertSourcePath, string fragSourcePath)
        {
            // Load a shader from a file
            var vertSource = File.ReadAllText(vertSourcePath);
            var fragSource = File.ReadAllText(fragSourcePath);
            return new Shader(gl, vertSource, fragSource);
        }

        private uint CreateShader(ShaderType type, string source)
        {
            // Create a GLShader
            var shader = _gl.CreateShader(type);
            _gl.ShaderSource(shader, source);
            _gl.CompileShader(shader);
            _gl.GetShader(shader, ShaderParameterName.CompileStatus, out int cStatus);
            if (cStatus != (int)GLEnum.True)
            {
                var log = _gl.GetShaderInfoLog(shader);
                throw new Exception($"Shader compiliation failed ({type}): {log}");
            }

            return shader;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing) { } // TODO: dispose managed state (managed objects)

                _gl.UseProgram(0);
                _gl.DeleteProgram(_handle);
                _disposedValue = true;
            }
        }

        ~Shader()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
