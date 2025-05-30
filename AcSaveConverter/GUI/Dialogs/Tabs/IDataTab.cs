using System;

namespace AcSaveConverter.GUI.Dialogs.Tabs
{
    public interface IDataTab : IDialog, INamed, IDisposable
    {
        public string DataType { get; }
        public void Load_Data(string path);
        public bool IsData(string path);
    }
}
