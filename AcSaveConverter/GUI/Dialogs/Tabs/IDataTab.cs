namespace AcSaveConverterImGui.GUI.Dialogs.Tabs
{
    public interface IDataTab : IDialog, INamed, IDisposable
    {
        public void Load_Data(string path);
        public bool IsData(string path);
    }
}
