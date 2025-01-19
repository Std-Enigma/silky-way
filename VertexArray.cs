using Silk.NET.OpenGL;

namespace Tutorial
{
    class VertexArray : IDisposable
    {
        private GL _gl;
        private uint _handle;
        private bool _disposedValue;

        public VertexArray(GL gl)
        {
            _gl = gl;
            _handle = gl.GenVertexArray();
            Bind();
        }

        public unsafe void VertexAttributePointer<TVertexType>(uint index, int count, VertexAttribPointerType type, uint vertexSize, int offSet) where TVertexType : unmanaged
        {
            // Setting up a vertex attribute pointer
            _gl.EnableVertexAttribArray(index);
            _gl.VertexAttribPointer(index, count, type, false, vertexSize * (uint)sizeof(TVertexType), (void*)(offSet * sizeof(TVertexType)));
        }

        public void Bind()
        {
            // Use this vertex array 
            _gl.BindVertexArray(_handle);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing) { }

                _gl?.BindVertexArray(0);
                _gl?.DeleteVertexArray(_handle);
                _disposedValue = true;
            }
        }

        ~VertexArray()
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
