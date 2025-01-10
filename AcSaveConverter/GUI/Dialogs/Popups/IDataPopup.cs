namespace AcSaveConverterImGui.GUI.Dialogs.Popups
{
    public interface IDataPopup : IDialog, IClosable, INamed, IDisposable
    {
        public void Load_Data(string path);
    }
}
