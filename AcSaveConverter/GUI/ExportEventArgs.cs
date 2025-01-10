namespace AcSaveConverterImGui.GUI
{
    public class ExportEventArgs : EventArgs
    {
        public string ExportName { get; set; }
        public GameType Game { get; set; }
        public PlatformType Platform { get; set; }
        public RegionType Region { get; set; }

        public ExportEventArgs(string exportName, GameType game, PlatformType platform, RegionType region)
        {
            ExportName = exportName;
            Game = game;
            Platform = platform;
            Region = region;
        }
    }
}
