using AcSaveConverter.Graphics;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace AcSaveConverter.Resources
{
    public class ResourceHandler : IDisposable
    {
        private readonly GuiTexturePool TexturePool;
        private readonly TextureHandle DefaultThumbnail;
        private bool disposedValue;

        public ResourceHandler(GuiTexturePool texturePool)
        {
            TexturePool = texturePool;
            DefaultThumbnail = TexturePool.LoadDDS(AssetPath.GetImagePath(Path.Combine("ArmoredCoreForAnswer", "background.dds")));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextureHandle GetDefaultThumbnail()
            => DefaultThumbnail;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextureHandle LoadDDS(byte[] bytes)
            => TexturePool.LoadDDS(bytes);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextureHandle LoadDDS(string path)
            => TexturePool.LoadDDS(path);

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DefaultThumbnail.Dispose();
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
