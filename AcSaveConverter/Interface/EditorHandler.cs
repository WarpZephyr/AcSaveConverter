using AcSaveConverter.Editors;
using AcSaveConverter.Editors.AcfaEditor;
using AcSaveConverter.Resources;
using System;
using System.Collections.Generic;

namespace AcSaveConverter.Interface
{
    public class EditorHandler : IDisposable
    {
        public readonly List<IEditorScreen> Editors;
        public IEditorScreen FocusedEditor;

        public AcfaEditorScreen AcfaEditor;
        private bool disposedValue;

        public EditorHandler(ResourceHandler resourceHandler)
        {
            AcfaEditor = new AcfaEditorScreen(resourceHandler);
            FocusedEditor = AcfaEditor;

            Editors =
            [
                AcfaEditor
            ];
        }

        public void Update(float deltaTime)
        {
            foreach (var editor in Editors)
            {
                editor.Update(deltaTime);
            }
        }

        public void OnMenuGui()
        {
            FocusedEditor.OnMenuGui();
        }

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var editor in Editors)
                    {
                        editor.Dispose();
                    }
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
