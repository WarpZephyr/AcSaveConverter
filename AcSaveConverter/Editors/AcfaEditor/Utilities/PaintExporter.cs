using AcSaveConverter.Resources;
using AcSaveFormats.ArmoredCoreForAnswer;
using AcSaveFormats.PlayStation3;
using System.IO;

namespace AcSaveConverter.Editors.AcfaEditor.Utilities
{
    internal static class PaintExporter
    {
        public static void Export(string folder, Paint paint, bool xbox, bool jp)
        {
            paint.Write(Path.Combine(folder, "PAINT.DAT"));

            if (xbox)
            {
                string thumbnailName = Path.Combine("ArmoredCoreForAnswer", "icon360.png");
                AssetPath.CopyImageTo(thumbnailName, "__thumbnail.png", folder);
            }
            else
            {
                string iconName = Path.Combine("ArmoredCoreForAnswer", "iconpnt.png");
                AssetPath.CopyImageTo(iconName, "ICON0.PNG", folder);

                string picName = Path.Combine("ArmoredCoreForAnswer", "pic1_paint.png");
                AssetPath.CopyImageTo(picName, "PIC1.PNG", folder);

                string subTitle;
                if (jp)
                {
                    subTitle = "ペイントデータ";
                }
                else
                {
                    subTitle = "Paint Data";
                }

                var sfo = BuildParamSfo(folder, subTitle);
                sfo.Write(Path.Combine(folder, "PARAM.SFO"));
            }
        }

        private static ParamSfo BuildParamSfo(string folder, string subTitle)
        {
            var sfo = new ParamSfo();
            var builder = new ParamSfoBuilder(sfo);
            builder.AddFile("PAINT.DAT", 1);
            builder.AddFile("ICON0.PNG", 0);
            builder.AddFile("PIC1.PNG", 0);
            builder.SetDefaultsRPCS3();
            builder.BuildRPCS3BLIST();

            builder.SetSaveDataDirectory(Path.GetFileName(folder));
            builder.SetTitle("ARMORED CORE for Answer");
            builder.SetSubTitle(subTitle);
            return sfo;
        }
    }
}
