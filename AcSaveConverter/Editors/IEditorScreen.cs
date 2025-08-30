using System;

namespace AcSaveConverter.Editors
{
    public interface IEditorScreen : IDisposable
    {
        public string EditorName { get; }
        public void Update(float deltaTime);
        public void OnMenuGui();
        public void OnGui();
    }
}
