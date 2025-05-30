using AcSaveConverter.Graphics.Textures;
using AcSaveConverter.Graphics;
using AcSaveConverter.GUI.Dialogs.Popups.ACFA;
using AcSaveConverter.GUI.Dialogs.Tabs;
using AcSaveConverter.IO;
using AcSaveConverter.IO.Assets;
using AcSaveFormats.ACFA;
using AcSaveFormats.ACFA.Designs;
using ImGuiNET;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using AcSaveConverter.Configuration;
using AcSaveConverter.Logging;

namespace AcSaveConverter.GUI.Dialogs.ACFA
{
    internal class DesignFaDialog : IDataTab
    {
        private readonly ImGuiGraphicsContext Graphics;
        private static ImGuiTexture? DefaultThumbnailCache;
        private bool IsDefaultThumbnail;

        public string Name { get; set; }

        public string DataType
            => "Design";

        private bool disposedValue;
        public bool IsDisposed
            => disposedValue;

        public Design Design { get; private set; }
        private ImGuiTexture ThumbnailCache;

        private readonly AcColorSetPopup ColorsPopup;

        public DesignFaDialog(string name, ImGuiGraphicsContext graphics, Design data)
        {
            Graphics = graphics;
            Name = name;

            Validate_Design(data);
            Design = data;

            if (DefaultThumbnailCache == null)
            {
                var defaultThumbnailPath = ImagesPath.GetImagePath(Path.Combine("ACFA", "thumb4026.bin"));
                var defaultThumbnail = Thumbnail.Read(defaultThumbnailPath, false);
                Design.Thumbnail = defaultThumbnail;
                DefaultThumbnailCache = graphics.TexturePool.LoadDDS(defaultThumbnail.GetDDSBytes());
            }

            ThumbnailCache = DefaultThumbnailCache;
            IsDefaultThumbnail = true;

            ColorsPopup = new AcColorSetPopup("AC Colors", Design.Colors);
        }

        #region Render

        public void Render()
        {
            ImGui.PushID(nameof(DesignFaDialog));
            Render_MenuBar();

            ThumbnailCache.Image(ThumbnailCache.Size);
            if (ImGui.BeginPopupContextItem("ThumbnailContextMenu"))
            {
                Render_ThumbnailContextMenu();

                ImGui.EndPopup();
            }

            ImGui.SameLine();

            ImGui.BeginGroup();
            Render_Design(Design, ColorsPopup);
            ImGui.EndGroup();

            ColorsPopup.Render();
            ImGui.PopID();
        }

        void Render_MenuBar()
        {
            if (ImGui.BeginMenuBar())
            {
                Render_FileMenu();
                ImGui.EndMenuBar();
            }
        }

        void Render_FileMenu()
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("Open"))
                {
                    string? file = FileDialog.OpenFile();
                    if (FileDialog.ValidFile(file))
                    {
                        Load_Data(file);
                    }
                }

                ImGui.EndMenu();
            }
        }

        internal static void Render_Design(Design design, AcColorSetPopup colorPopup)
        {
            ImGui.SeparatorText(design.DesignName);
            if (ImGui.BeginTable("DesignTable", 2, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.BordersInnerV))
            {
                ImGui.TableNextColumn();
                ImGui.Text("Design");
                ImGui.TableNextColumn();
                ImGui.Text(design.DesignName);
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text("Designer");
                ImGui.TableNextColumn();
                ImGui.Text(design.DesignerName);
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text("Creation Time");
                ImGui.TableNextColumn();
                ImGui.Text(design.CreationTimeStamp.ToString());
                ImGui.EndTable();
            }

            if (ImGui.Button("AC Colors"))
            {
                colorPopup.Load_Data(design.Colors);
                colorPopup.OpenPopup = true;
            }
        }

        void Render_ThumbnailContextMenu()
        {
            if (ImGui.Button("Export"))
            {
                const StringComparison comp = StringComparison.InvariantCultureIgnoreCase;
                string? path = FileDialog.GetSaveFilePath("png;jpg;bmp;dds");
                if (FileDialog.ValidSavePath(path))
                {
                    if (path.EndsWith(".png", comp))
                    {
                        TextureSave.ExportPng(path, Design.Thumbnail.GetDDSBytes());
                    }
                    else if (path.EndsWith(".jpg", comp) || path.EndsWith(".jpeg", comp))
                    {
                        TextureSave.ExportJpeg(path, Design.Thumbnail.GetDDSBytes());
                    }
                    else if (path.EndsWith(".bmp", comp))
                    {
                        TextureSave.ExportBmp(path, Design.Thumbnail.GetDDSBytes());
                    }
                    else if (path.EndsWith(".dds", comp))
                    {
                        TextureSave.ExportDDS(path, Design.Thumbnail.GetDDSBytes());
                    }
                }
            }

            if (ImGui.Button("Import"))
            {
                try
                {
                    string? path = FileDialog.OpenFile("dds;bin");
                    if (FileDialog.ValidFile(path))
                    {
                        if (ImportThumbnail(path, AppConfig.Instance.Xbox360, Design.Thumbnail, out Thumbnail? output))
                        {
                            Design.Thumbnail = output;
                            RebuildThumbnailCache();
                        }
                    }
                }
                catch
                {
                    
                }
            }
        }

        #endregion

        #region Data

        public void Load_Data(Design data)
        {
            Validate_Design(data);
            Design = data;
            RebuildThumbnailCache();
            ColorsPopup.Load_Data(Design.Colors);
        }

        public void Load_Data(string path)
        {
            try
            {
                Log.WriteLine($"Loading {DataType} from path: \"{path}\"");
                Load_Data(Design.Read(path, DetectUTF16(path), DetectXbox360(path)));
            }
            catch (Exception ex)
            {
                Log.WriteLine($"Failed to load {DataType} from path \"{path}\": {ex}");
            }
        }

        public bool IsData(string path)
        {
            return path.EndsWith("ACDES.DAT", StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion

        #region Detection

        private static bool DetectUTF16(string file)
        {
            if (AppConfig.Instance.AutoDetectEncoding)
            {
                var fileInfo = new FileInfo(file);
                long length = fileInfo.Length;
                if (length == 24280)
                {
                    // A 2-byte encoding is being used
                    // Which should be UTF16
                    return true;
                }
                else if (length == 24184)
                {
                    // A 1-byte encoding is being used
                    // Which should be ShiftJIS
                    return false;
                }
            }

            // Encoding state could not be determined automatically or we choose to manually override it
            return AppConfig.Instance.UTF16;
        }

        private static bool DetectXbox360(string file)
        {
            // Design data won't be purely alone on Xbox 360 like it is on PS3
            return false;
        }

        #endregion

        #region Validation

        private static void Validate_Tuning(Design.DesignTuning tuning)
        {
            tuning.Unk1C = 0;
            tuning.Unk1D = 0;
            tuning.Unk1E = 0;
            tuning.Unk1F = 0;

            byte[] tunes =
            [
                tuning.EnOutput,
                tuning.EnCapacity,
                tuning.KpOutput,
                tuning.Load,
                tuning.EnWeaponSkill,
                tuning.Maneuverability,
                tuning.FiringStability,
                tuning.AimPrecision,
                tuning.LockSpeed,
                tuning.MissileLockSpeed,
                tuning.RadarRefreshRate,
                tuning.EcmResistance,
                tuning.RectificationHead,
                tuning.RectificationCore,
                tuning.RectificationArm,
                tuning.RectificationLeg,
                tuning.HorizontalThrustMain,
                tuning.VerticalThrust,
                tuning.HorizontalThrustSide,
                tuning.HorizontalThrustBack,
                tuning.QuickBoostMain,
                tuning.QuickBoostSide,
                tuning.QuickBoostBack,
                tuning.OveredBoostThrust,
                tuning.TurningAbility,
                tuning.StabilityHead,
                tuning.StabilityCore,
                tuning.StabilityLegs,
                tuning.Unk1C,
                tuning.Unk1D,
                tuning.Unk1E,
                tuning.Unk1F
            ];

            const byte minTune = 0;
            const byte maxTune = 50;
            const int maxFrs = 442;
            var totalFrsConsumed = 0;
            for (int i = 0; i < tunes.Length; i++)
            {
                byte value = tunes[i];
                byte clamped = Math.Clamp(value, minTune, maxTune);
                if (value != clamped)
                {
                    Log.WriteLine($"Detected invalid tune value range: {value}");
                    Log.WriteLine($"Clamping invalid tune value to: {clamped}");
                }

                tunes[i] = clamped;
                totalFrsConsumed += tunes[i];
            }

            int remaining = totalFrsConsumed - maxFrs;
            if (remaining > 0)
            {
                Log.WriteLine($"Detected overtuning with a total FRS consumption of: {totalFrsConsumed}");
                Log.WriteLine("Removing tuning points until balanced.");

                for (int i = tunes.Length - 1; i >= 0; i--)
                {
                    if (tunes[i] == maxTune)
                    {
                        if (remaining >= maxTune)
                        {
                            remaining -= maxTune;
                            tunes[i] = minTune;
                        }
                        else
                        {
                            tunes[i] -= (byte)remaining;
                            remaining = 0;
                        }
                    }
                    else if (tunes[i] > minTune && tunes[i] < maxTune)
                    {
                        if (remaining >= tunes[i])
                        {
                            remaining -= tunes[i];
                            tunes[i] = minTune;
                        }
                        else
                        {
                            tunes[i] -= (byte)remaining;
                            remaining = 0;
                        }
                    }
                }
            }

            int tuneIndex = 0;
            tuning.EnOutput = tunes[tuneIndex++];
            tuning.EnCapacity = tunes[tuneIndex++];
            tuning.KpOutput = tunes[tuneIndex++];
            tuning.Load = tunes[tuneIndex++];
            tuning.EnWeaponSkill = tunes[tuneIndex++];
            tuning.Maneuverability = tunes[tuneIndex++];
            tuning.FiringStability = tunes[tuneIndex++];
            tuning.AimPrecision = tunes[tuneIndex++];
            tuning.LockSpeed = tunes[tuneIndex++];
            tuning.MissileLockSpeed = tunes[tuneIndex++];
            tuning.RadarRefreshRate = tunes[tuneIndex++];
            tuning.EcmResistance = tunes[tuneIndex++];
            tuning.RectificationHead = tunes[tuneIndex++];
            tuning.RectificationCore = tunes[tuneIndex++];
            tuning.RectificationArm = tunes[tuneIndex++];
            tuning.RectificationLeg = tunes[tuneIndex++];
            tuning.HorizontalThrustMain = tunes[tuneIndex++];
            tuning.VerticalThrust = tunes[tuneIndex++];
            tuning.HorizontalThrustSide = tunes[tuneIndex++];
            tuning.HorizontalThrustBack = tunes[tuneIndex++];
            tuning.QuickBoostMain = tunes[tuneIndex++];
            tuning.QuickBoostSide = tunes[tuneIndex++];
            tuning.QuickBoostBack = tunes[tuneIndex++];
            tuning.OveredBoostThrust = tunes[tuneIndex++];
            tuning.TurningAbility = tunes[tuneIndex++];
            tuning.StabilityHead = tunes[tuneIndex++];
            tuning.StabilityCore = tunes[tuneIndex++];
            tuning.StabilityLegs = tunes[tuneIndex++];
            tuning.Unk1C = tunes[tuneIndex++];
            tuning.Unk1D = tunes[tuneIndex++];
            tuning.Unk1E = tunes[tuneIndex++];
            tuning.Unk1F = tunes[tuneIndex];
        }

        private static void Validate_Parts(Design.DesignParts parts)
        {
            static ushort ValidatePart(ushort part, ushort defaultValue)
            {
                const ushort debugPartRange = 9000;
                if (part >= debugPartRange)
                {
                    Log.WriteLine($"Detected debug part ID: {part}");
                    Log.WriteLine($"Changing debug part ID to default: {defaultValue}");
                    return defaultValue;
                }

                return part;
            }

            parts.Head = ValidatePart(parts.Head, 2010);
            parts.Core = ValidatePart(parts.Core, 2010);
            parts.Arms = ValidatePart(parts.Arms, 2010);
            parts.Legs = ValidatePart(parts.Legs, 2010);
            parts.Fcs = ValidatePart(parts.Fcs, 2010);
            parts.Generator = ValidatePart(parts.Generator, 2010);
            parts.MainBooster = ValidatePart(parts.MainBooster, 2020);
            parts.BackBooster = ValidatePart(parts.BackBooster, 2010);
            parts.SideBooster = ValidatePart(parts.SideBooster, 2020);
            parts.OveredBooster = ValidatePart(parts.OveredBooster, 2010);
            parts.RightArmUnit = ValidatePart(parts.RightArmUnit, 2020);
            parts.LeftArmUnit = ValidatePart(parts.LeftArmUnit, 2040);
            parts.RightBackUnit = ValidatePart(parts.RightBackUnit, 0);
            parts.LeftBackUnit = ValidatePart(parts.LeftBackUnit, 2020);
            parts.ShoulderUnit = ValidatePart(parts.ShoulderUnit, 0);
            parts.RightHangarUnit = ValidatePart(parts.RightHangarUnit, 0);
            parts.LeftHangarUnit = ValidatePart(parts.LeftHangarUnit, 0);
            parts.StabilizerHeadTop = ValidatePart(parts.StabilizerHeadTop, 0);
            parts.StabilizerHeadRight = ValidatePart(parts.StabilizerHeadRight, 0);
            parts.StabilizerHeadLeft = ValidatePart(parts.StabilizerHeadLeft, 0);
            parts.StabilizerCoreUpperRight = ValidatePart(parts.StabilizerCoreUpperRight, 0);
            parts.StabilizerCoreUpperLeft = ValidatePart(parts.StabilizerCoreUpperLeft, 0);
            parts.StabilizerCoreLowerRight = ValidatePart(parts.StabilizerCoreLowerRight, 0);
            parts.StabilizerCoreLowerLeft = ValidatePart(parts.StabilizerCoreLowerLeft, 0);
            parts.StabilizerArmRight = ValidatePart(parts.StabilizerArmRight, 0);
            parts.StabilizerArmLeft = ValidatePart(parts.StabilizerArmLeft, 0);
            parts.StabilizerLegsBack = ValidatePart(parts.StabilizerLegsBack, 0);
            parts.StabilizerLegsUpperRight = ValidatePart(parts.StabilizerLegsUpperRight, 0);
            parts.StabilizerLegsUpperLeft = ValidatePart(parts.StabilizerLegsUpperLeft, 0);
            parts.StabilizerLegsUpperRightBack = ValidatePart(parts.StabilizerLegsUpperRightBack, 0);
            parts.StabilizerLegsUpperLeftBack = ValidatePart(parts.StabilizerLegsUpperLeftBack, 0);
            parts.StabilizerLegsMiddleRight = ValidatePart(parts.StabilizerLegsMiddleRight, 0);
            parts.StabilizerLegsMiddleLeft = ValidatePart(parts.StabilizerLegsMiddleLeft, 0);
            parts.StabilizerLegsMiddleRightBack = ValidatePart(parts.StabilizerLegsMiddleRightBack, 0);
            parts.StabilizerLegsMiddleLeftBack = ValidatePart(parts.StabilizerLegsMiddleLeftBack, 0);
            parts.StabilizerLegsLowerRight = ValidatePart(parts.StabilizerLegsLowerRight, 0);
            parts.StabilizerLegsLowerLeft = ValidatePart(parts.StabilizerLegsLowerLeft, 0);
            parts.StabilizerLegsLowerRightBack = ValidatePart(parts.StabilizerLegsLowerRightBack, 0);
            parts.StabilizerLegsLowerLeftBack = ValidatePart(parts.StabilizerLegsLowerLeftBack, 0);
        }

        internal static void Validate_Design(Design design)
        {
            Log.WriteLine($"Validating design {design.DesignName} by {design.DesignerName}.");

            var tuning = design.Tuning;
            Validate_Tuning(tuning);
            Validate_Parts(design.Parts);
        }

        #endregion

        #region Util

        private static string GetSecondsTimeString(float seconds)
        {
            float hour = seconds / 3600;
            float minute = seconds / 60 % 60;
            float second = seconds % 60;
            return $"{hour:00}:{minute:00}:{second:00}";
        }

        #endregion

        #region Thumbnail

        void RebuildThumbnailCache()
        {
            InvalidateThumbnailCache();
            ThumbnailCache = Graphics.TexturePool.LoadDDS(Design.Thumbnail.GetDDSBytes());
            IsDefaultThumbnail = false;
        }

        void InvalidateThumbnailCache()
        {
            if (!IsDefaultThumbnail)
            {
                ThumbnailCache.Dispose();
            }
        }

        public static void InvalidateDefaultThumbnailCache()
        {
            DefaultThumbnailCache?.Dispose();
        }

        public static bool ImportThumbnail(string path, bool xbox, Thumbnail previous, [NotNullWhen(true)] out Thumbnail? output)
        {
            if (path.EndsWith(".dds", StringComparison.InvariantCultureIgnoreCase))
            {
                byte[] bytes = File.ReadAllBytes(path);
                DDS dds = DDS.Read(bytes);
                int dataOffset = dds.DataOffset;
                if (dds.Width != 256
                 || dds.Height != 128
                 || dds.HeaderDXT10 != null
                 || dds.Flags != (DDS.DDSD.CAPS | DDS.DDSD.HEIGHT | DDS.DDSD.WIDTH | DDS.DDSD.PIXELFORMAT | DDS.DDSD.MIPMAPCOUNT | DDS.DDSD.LINEARSIZE)
                 || dds.Caps != DDS.DDSCAPS.TEXTURE
                 || dds.MipMapCount != 1
                 || dds.DDSPixelFormat.FourCC != "DXT1"
                 || (bytes.Length - dataOffset) != 16384
                 || !previous.SetPixelData(bytes[dataOffset..]))
                {
                    // Currently cannot convert to the desired format
                    output = null;
                    return false;
                }

                output = previous;
            }
            else if (path.EndsWith(".bin"))
            {
                try
                {
                    output = Thumbnail.Read(path, xbox);
                }
                catch
                {
                    // Was not a thumbnail file
                    output = null;
                    return false;
                }
            }
            else
            {
                // Was not a loadable file
                output = null;
                return false;
            }

            return true;
        }

        #endregion

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    InvalidateThumbnailCache();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
