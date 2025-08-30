using AcSaveConverter.Configuration;
using System;

namespace AcSaveConverter.Editors.AcfaEditor.Data
{
    public class ExportEventArgs : EventArgs
    {
        public string ExportName { get; set; }
        public GameType Game { get; set; }
        public PlatformType Platform { get; set; }
        public RegionType Region { get; set; }
        public ExportKind Kind { get; set; }

        public ExportEventArgs(string exportName, GameType game, PlatformType platform, RegionType region, ExportKind kind)
        {
            ExportName = exportName;
            Game = game;
            Platform = platform;
            Region = region;
            Kind = kind;
        }
    }
}
