using ImGuiNET;
using System;
using System.Numerics;
using Veldrid;

namespace AcSaveConverter.Graphics
{
    public class ImGuiTexture : IDisposable
    {
        private readonly ImGuiTexturePool Pool;
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
        
        internal ImGuiTexture(ImGuiTexturePool pool, Texture texture, nint handle)
        {
            Pool = pool;
            Texture = texture;
            Size = new Vector2(texture.Width, texture.Height);
            Handle = handle;
        }

        #region Image

        public void Image(Vector2 size)
            => ImGui.Image(Handle, size);

        public void Image(Vector2 size, Vector2 uv0)
            => ImGui.Image(Handle, size, uv0);

        public void Image(Vector2 size, Vector2 uv0, Vector2 uv1)
            => ImGui.Image(Handle, size, uv0, uv1);

        public void Image(Vector2 size, Vector2 uv0, Vector2 uv1, Vector4 tint_col)
            => ImGui.Image(Handle, size, uv0, uv1, tint_col);

        public void Image(Vector2 size, Vector2 uv0, Vector2 uv1, Vector4 tint_col, Vector4 border_col)
            => ImGui.Image(Handle, size, uv0, uv1, tint_col, border_col);

        #endregion

        #region Image Button

        public bool ImageButton(string str_id, Vector2 size)
            => ImGui.ImageButton(str_id, Handle, size);

        public bool ImageButton(string str_id, Vector2 size, Vector2 uv0)
            => ImGui.ImageButton(str_id, Handle, size, uv0);

        public bool ImageButton(string str_id, Vector2 size, Vector2 uv0, Vector2 uv1)
           => ImGui.ImageButton(str_id, Handle, size, uv0, uv1);

        public bool ImageButton(string str_id, Vector2 size, Vector2 uv0, Vector2 uv1, Vector4 bg_col)
           => ImGui.ImageButton(str_id, Handle, size, uv0, uv1, bg_col);

        public bool ImageButton(string str_id, Vector2 size, Vector2 uv0, Vector2 uv1, Vector4 bg_col, Vector4 tint_col)
           => ImGui.ImageButton(str_id, Handle, size, uv0, uv1, bg_col, tint_col);

        #endregion

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
