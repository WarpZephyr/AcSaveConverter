using System;

namespace AcSaveConverter.GUI.Dialogs.Popups
{
    public interface IDataPopup : IDialog, IClosable, INamed, IDisposable
    {
        public void Load_Data(string path);
    }
}
