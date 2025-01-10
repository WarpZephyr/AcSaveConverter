namespace AcSaveConverterImGui.Graphics
{
    public class UI
    {
        public string FontEnglish { get; set; }
        public string FontOther { get; set; }
        public bool UseFontChinese { get; set; }
        public bool UseFontKorean { get; set; }
        public bool UseFontThai { get; set; }
        public bool UseFontVietnamese { get; set; }
        public bool UseFontCyrillic { get; set; }
        public bool ScaleByDPI { get; set; }
        public float InterfaceFontSize { get; set; }
        public float UIScale { get; set; }

        internal UI()
        {
            FontEnglish = "RobotoMono-Light.ttf";
            FontOther = "NotoSansCJKtc-Light.otf";
            InterfaceFontSize = 13.0f;
            UIScale = 1.0f;
        }
    }
}
