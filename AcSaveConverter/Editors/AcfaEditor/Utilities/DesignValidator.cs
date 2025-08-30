using AcSaveConverter.Logging;
using AcSaveFormats.ArmoredCoreForAnswer;
using System;
using System.Text;

namespace AcSaveConverter.Editors.AcfaEditor.Utilities
{
    internal static class DesignValidator
    {
        [Flags]
        private enum ValidationFailureFlags
        {
            None = 0,
            OverTuning,
            TooMuchFrs,
            DebugParts
        }

        private static void ValidateTuning(Design.DesignTuning tuning, ref ValidationFailureFlags validationFlags)
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
                    validationFlags |= ValidationFailureFlags.OverTuning;
                }

                tunes[i] = clamped;
                totalFrsConsumed += tunes[i];
            }

            int remaining = totalFrsConsumed - maxFrs;
            if (remaining > 0)
            {
                validationFlags |= ValidationFailureFlags.TooMuchFrs;

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

        private static void ValidateParts(Design.DesignParts parts, ref ValidationFailureFlags validationFlags)
        {
            bool hadDebugPart = false;
            ushort ValidatePart(ushort part, ushort defaultValue)
            {
                const ushort debugPartRange = 9000;
                if (part >= debugPartRange)
                {
                    hadDebugPart = true;
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

            if (hadDebugPart)
            {
                validationFlags |= ValidationFailureFlags.DebugParts;
            }
        }

        internal static void Validate(Design design)
        {
            var flags = ValidationFailureFlags.None;
            ValidateTuning(design.Tuning, ref flags);
            ValidateParts(design.Parts, ref flags);
            if (flags != ValidationFailureFlags.None)
            {
                var sb = new StringBuilder();
                if ((flags & ValidationFailureFlags.OverTuning) != 0)
                {
                    sb.Append("OverTuning");
                }

                if ((flags & ValidationFailureFlags.TooMuchFrs) != 0)
                {
                    if (sb.Length > 0)
                        sb.Append('|');
                    sb.Append("TooMuchFrs");
                }

                if ((flags & ValidationFailureFlags.DebugParts) != 0)
                {
                    if (sb.Length > 0)
                        sb.Append('|');
                    sb.Append("DebugParts");
                }

                Log.WriteLine($"{design.DesignName} by {design.DesignerName} validation failed and was corrected due to: {sb}");
            }
        }

        internal static void Validate(DesignDocument designDocument)
        {
            foreach (var design in designDocument.Designs)
            {
                Validate(design);
            }
        }
    }
}
