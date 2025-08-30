namespace AcSaveConverter.Configuration
{
    internal interface IConfig
    {
        public int GetConfigVersion();
        public int GetCurrentVersion();
        public void SetConfigVersion(int version);
        public void Save();
    }
}
