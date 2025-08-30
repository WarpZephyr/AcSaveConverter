using AcSaveConverter.Interface;
using AcSaveFormats.ArmoredCoreForAnswer.Colors;
using ImGuiNET;
using System;
using System.Drawing;

namespace AcSaveConverter.Editors.AcfaEditor.Popups
{
    public class AcColorSetPopup
    {
        public bool IsOpen { get; set; }
        public bool OpenPopup { get; set; }
        private AcColorSet Data;

        public AcColorSetPopup()
        {
            Data = new AcColorSet();
        }

        public void Display()
        {
            ImGui.PushID(nameof(ExportPopup));

            if (OpenPopup)
            {
                ImGui.OpenPopup("AC Colors");
                IsOpen = true;
                OpenPopup = false;
            }

            bool open = IsOpen;
            if (ImGui.BeginPopupModal("AC Colors", ref open))
            {
                ShowPopup();
                ImGui.EndPopup();
            }

            IsOpen = open;
            ImGui.PopID();
        }

        private void ShowPopup()
        {
            Data.HeadColor = ShowColorSetRow("Head", Data.HeadColor);
            Data.CoreColor = ShowColorSetRow("Core", Data.CoreColor);
            Data.ArmRightColor = ShowColorSetRow("Right Arm", Data.ArmRightColor);
            Data.ArmLeftColor = ShowColorSetRow("Left Arm", Data.ArmLeftColor);
            Data.LegsColor = ShowColorSetRow("Legs", Data.LegsColor);
            Data.ArmUnitRightColor = ShowColorSetRow("Right Arm Unit", Data.ArmUnitRightColor);
            Data.ArmUnitLeftColor = ShowColorSetRow("Left Arm Unit", Data.ArmUnitLeftColor);
            Data.BackUnitRightColor = ShowColorSetRow("Right Back Unit", Data.BackUnitRightColor);
            Data.BackUnitLeftColor = ShowColorSetRow("Left Back Unit", Data.BackUnitLeftColor);
            Data.ShoulderUnitColor = ShowColorSetRow("Shoulder Unit", Data.ShoulderUnitColor);
            Data.HangerUnitRightColor = ShowColorSetRow("Right Hanger Unit", Data.HangerUnitRightColor);
            Data.HangerUnitLeftColor = ShowColorSetRow("Left Hanger Unit", Data.HangerUnitLeftColor);
            Data.StabilizerHeadTopColor = ShowColorSetRow("Stabilizer: Head Top", Data.StabilizerHeadTopColor);
            Data.StabilizerHeadRightColor = ShowColorSetRow("Stabilizer: Head Right", Data.StabilizerHeadRightColor);
            Data.StabilizerHeadLeftColor = ShowColorSetRow("Stabilizer: Head Left", Data.StabilizerHeadLeftColor);
            Data.StabilizerCoreUpperRightColor = ShowColorSetRow("Stabilizer: Core Upper Right", Data.StabilizerCoreUpperRightColor);
            Data.StabilizerCoreUpperLeftColor = ShowColorSetRow("Stabilizer: Core Upper Left", Data.StabilizerCoreUpperLeftColor);
            Data.StabilizerCoreLowerRightColor = ShowColorSetRow("Stabilizer: Core Lower Right", Data.StabilizerCoreLowerRightColor);
            Data.StabilizerCoreLowerLeftColor = ShowColorSetRow("Stabilizer: Core Lower Left", Data.StabilizerCoreLowerLeftColor);
            Data.StabilizerArmRightColor = ShowColorSetRow("Stabilizer: Right Arm", Data.StabilizerArmRightColor);
            Data.StabilizerArmLeftColor = ShowColorSetRow("Stabilizer: Left Arm", Data.StabilizerArmLeftColor);
            Data.StabilizerLegsBackColor = ShowColorSetRow("Stabilizer: Legs Back", Data.StabilizerLegsBackColor);
            Data.StabilizerLegsUpperRightColor = ShowColorSetRow("Stabilizer: Legs Upper Right", Data.StabilizerLegsUpperRightColor);
            Data.StabilizerLegsUpperLeftColor = ShowColorSetRow("Stabilizer: Legs Upper Left", Data.StabilizerLegsUpperLeftColor);
            Data.StabilizerLegsUpperRightBackColor = ShowColorSetRow("Stabilizer: Legs Upper Right Back", Data.StabilizerLegsUpperRightBackColor);
            Data.StabilizerLegsUpperLeftBackColor = ShowColorSetRow("Stabilizer: Legs Upper Left Back", Data.StabilizerLegsUpperLeftBackColor);
            Data.StabilizerLegsMiddleRightColor = ShowColorSetRow("Stabilizer: Legs Middle Right", Data.StabilizerLegsMiddleRightColor);
            Data.StabilizerLegsMiddleLeftColor = ShowColorSetRow("Stabilizer: Legs Middle Left", Data.StabilizerLegsMiddleLeftColor);
            Data.StabilizerLegsMiddleRightBackColor = ShowColorSetRow("Stabilizer: Legs Middle Right Back", Data.StabilizerLegsMiddleRightBackColor);
            Data.StabilizerLegsMiddleLeftBackColor = ShowColorSetRow("Stabilizer: Legs Middle Left Back", Data.StabilizerLegsMiddleLeftBackColor);
            Data.StabilizerLegsLowerRightColor = ShowColorSetRow("Stabilizer: Legs Lower Right", Data.StabilizerLegsLowerRightColor);
            Data.StabilizerLegsLowerLeftColor = ShowColorSetRow("Stabilizer: Legs Lower Left", Data.StabilizerLegsLowerLeftColor);
            Data.StabilizerLegsLowerRightBackColor = ShowColorSetRow("Stabilizer: Legs Lower Right Back", Data.StabilizerLegsLowerRightBackColor);
            Data.StabilizerLegsLowerLeftBackColor = ShowColorSetRow("Stabilizer: Legs Lower Left Back", Data.StabilizerLegsLowerLeftBackColor);
        }

        private static ColorSet ShowColorSetRow(string label, ColorSet colorset)
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

        public void Load(AcColorSet data)
        {
            Data = data;
        }
    }
}
