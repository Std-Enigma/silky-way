using Silk.NET.OpenGL;

namespace Tutorial
{
    class BufferObject<TDataType> : IDisposable where TDataType : unmanaged
    {
        private GL _gl;
        private BufferTargetARB _target;
        private uint _handle;
        private bool _disposedValue;

        public unsafe BufferObject(GL gl, BufferTargetARB target, Span<TDataType> data)
        {
            _gl = gl;
            _target = target;
            _handle = _gl.GenBuffer();
            // Use this buffer and the set the data
            Bind();
            fixed (TDataType* buf = data)
                _gl.BufferData(target, (nuint)(data.Length * sizeof(TDataType)), buf, BufferUsageARB.StaticDraw);
        }

        public void Bind()
        {
            // Bind the buffer
            _gl.BindBuffer(_target, _handle);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing) { }

                _gl.BindBuffer(_target, _handle);
                _gl.DeleteBuffer(_handle);
                _disposedValue = true;
            }
        }

        ~BufferObject()
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
