using AcSaveFormats.ACFA.Colors;
using ImGuiNET;
using System;
using System.Drawing;

namespace AcSaveConverterImGui.GUI.Dialogs.Popups.ACFA
{
    public class AcColorSetPopup : IDataPopup
    {
        public bool Open { get; set; }
        public bool OpenPopup { get; set; }

        public AcColorSet AcColorSet { get; set; }
        public string Name { get; set; }

        public AcColorSetPopup(string name, AcColorSet acColorSet)
        {
            Name = name;
            AcColorSet = acColorSet;
        }

        #region Render

        public void Render()
        {
            ImGui.PushID(nameof(AcColorSetPopup));
            if (OpenPopup)
            {
                ImGui.OpenPopup("AC Colors");
                Open = true;
                OpenPopup = false;
            }

            bool open = Open;
            if (ImGui.BeginPopupModal("AC Colors", ref open))
            {
                Render_AcColorSet();
                ImGui.EndPopup();
            }

            Open = open;
            ImGui.PopID();
        }

        void Render_AcColorSet()
        {
            AcColorSet.HeadColor = Render_ColorSetRow("Head", AcColorSet.HeadColor);
            AcColorSet.CoreColor = Render_ColorSetRow("Core", AcColorSet.CoreColor);
            AcColorSet.ArmRightColor = Render_ColorSetRow("Right Arm", AcColorSet.ArmRightColor);
            AcColorSet.ArmLeftColor = Render_ColorSetRow("Left Arm", AcColorSet.ArmLeftColor);
            AcColorSet.LegsColor = Render_ColorSetRow("Legs", AcColorSet.LegsColor);
            AcColorSet.ArmUnitRightColor = Render_ColorSetRow("Right Arm Unit", AcColorSet.ArmUnitRightColor);
            AcColorSet.ArmUnitLeftColor = Render_ColorSetRow("Left Arm Unit", AcColorSet.ArmUnitLeftColor);
            AcColorSet.BackUnitRightColor = Render_ColorSetRow("Right Back Unit", AcColorSet.BackUnitRightColor);
            AcColorSet.BackUnitLeftColor = Render_ColorSetRow("Left Back Unit", AcColorSet.BackUnitLeftColor);
            AcColorSet.ShoulderUnitColor = Render_ColorSetRow("Shoulder Unit", AcColorSet.ShoulderUnitColor);
            AcColorSet.HangerUnitRightColor = Render_ColorSetRow("Right Hanger Unit", AcColorSet.HangerUnitRightColor);
            AcColorSet.HangerUnitLeftColor = Render_ColorSetRow("Left Hanger Unit", AcColorSet.HangerUnitLeftColor);
            AcColorSet.StabilizerHeadTopColor = Render_ColorSetRow("Stabilizer: Head Top", AcColorSet.StabilizerHeadTopColor);
            AcColorSet.StabilizerHeadRightColor = Render_ColorSetRow("Stabilizer: Head Right", AcColorSet.StabilizerHeadRightColor);
            AcColorSet.StabilizerHeadLeftColor = Render_ColorSetRow("Stabilizer: Head Left", AcColorSet.StabilizerHeadLeftColor);
            AcColorSet.StabilizerCoreUpperRightColor = Render_ColorSetRow("Stabilizer: Core Upper Right", AcColorSet.StabilizerCoreUpperRightColor);
            AcColorSet.StabilizerCoreUpperLeftColor = Render_ColorSetRow("Stabilizer: Core Upper Left", AcColorSet.StabilizerCoreUpperLeftColor);
            AcColorSet.StabilizerCoreLowerRightColor = Render_ColorSetRow("Stabilizer: Core Lower Right", AcColorSet.StabilizerCoreLowerRightColor);
            AcColorSet.StabilizerCoreLowerLeftColor = Render_ColorSetRow("Stabilizer: Core Lower Left", AcColorSet.StabilizerCoreLowerLeftColor);
            AcColorSet.StabilizerArmRightColor = Render_ColorSetRow("Stabilizer: Right Arm", AcColorSet.StabilizerArmRightColor);
            AcColorSet.StabilizerArmLeftColor = Render_ColorSetRow("Stabilizer: Left Arm", AcColorSet.StabilizerArmLeftColor);
            AcColorSet.StabilizerLegsBackColor = Render_ColorSetRow("Stabilizer: Legs Back", AcColorSet.StabilizerLegsBackColor);
            AcColorSet.StabilizerLegsUpperRightColor = Render_ColorSetRow("Stabilizer: Legs Upper Right", AcColorSet.StabilizerLegsUpperRightColor);
            AcColorSet.StabilizerLegsUpperLeftColor = Render_ColorSetRow("Stabilizer: Legs Upper Left", AcColorSet.StabilizerLegsUpperLeftColor);
            AcColorSet.StabilizerLegsUpperRightBackColor = Render_ColorSetRow("Stabilizer: Legs Upper Right Back", AcColorSet.StabilizerLegsUpperRightBackColor);
            AcColorSet.StabilizerLegsUpperLeftBackColor = Render_ColorSetRow("Stabilizer: Legs Upper Left Back", AcColorSet.StabilizerLegsUpperLeftBackColor);
            AcColorSet.StabilizerLegsMiddleRightColor = Render_ColorSetRow("Stabilizer: Legs Middle Right", AcColorSet.StabilizerLegsMiddleRightColor);
            AcColorSet.StabilizerLegsMiddleLeftColor = Render_ColorSetRow("Stabilizer: Legs Middle Left", AcColorSet.StabilizerLegsMiddleLeftColor);
            AcColorSet.StabilizerLegsMiddleRightBackColor = Render_ColorSetRow("Stabilizer: Legs Middle Right Back", AcColorSet.StabilizerLegsMiddleRightBackColor);
            AcColorSet.StabilizerLegsMiddleLeftBackColor = Render_ColorSetRow("Stabilizer: Legs Middle Left Back", AcColorSet.StabilizerLegsMiddleLeftBackColor);
            AcColorSet.StabilizerLegsLowerRightColor = Render_ColorSetRow("Stabilizer: Legs Lower Right", AcColorSet.StabilizerLegsLowerRightColor);
            AcColorSet.StabilizerLegsLowerLeftColor = Render_ColorSetRow("Stabilizer: Legs Lower Left", AcColorSet.StabilizerLegsLowerLeftColor);
            AcColorSet.StabilizerLegsLowerRightBackColor = Render_ColorSetRow("Stabilizer: Legs Lower Right Back", AcColorSet.StabilizerLegsLowerRightBackColor);
            AcColorSet.StabilizerLegsLowerLeftBackColor = Render_ColorSetRow("Stabilizer: Legs Lower Left Back", AcColorSet.StabilizerLegsLowerLeftBackColor);
        }

        static ColorSet Render_ColorSetRow(string label, ColorSet colorset)
        {
            Span<Color> colorSpan = [colorset.Main, colorset.Sub, colorset.Support, colorset.Optional, colorset.Joint, colorset.Device];
            if (ImGuiEx.ColorSetEdit4(label, colorSpan))
            {
                colorset.Main = colorSpan[0];
                colorset.Sub = colorSpan[1];
                colorset.Support = colorSpan[2];
                colorset.Optional = colorSpan[3];
                colorset.Joint = colorSpan[4];
                colorset.Device = colorSpan[5];
            }

            return colorset;
        }

        #endregion

        #region Data

        public void Load_Data(AcColorSet data)
        {
            AcColorSet = data;
        }

        public void Load_Data(string path)
        {
            Load_Data(AcColorSet.Read(path));
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
