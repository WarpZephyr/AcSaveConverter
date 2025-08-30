using System;
using System.Numerics;
using Veldrid;

namespace AcSaveConverter.Graphics
{
    public class TextureHandle : IDisposable
    {
        private readonly GuiTexturePool Pool;
        internal readonly Texture Texture;
        public readonly Vector2 Size;
        private nint Handle;
        private bool disposedValue;

        public uint Width
            => Texture.Width;

        public uint Height
            => Texture.Height;

        public bool IsDisposed
            => disposedValue;

        internal TextureHandle(GuiTexturePool pool, Texture texture, nint handle)
        {
            Pool = pool;
            Texture = texture;
            Size = new Vector2(texture.Width, texture.Height);
            Handle = handle;
        }

        public nint GetHandle()
            => Handle;

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Pool.DestroyTexture(this);
                    Handle = IntPtr.Zero;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
