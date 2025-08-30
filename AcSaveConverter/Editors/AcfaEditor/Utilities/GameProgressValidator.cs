using AcSaveConverter.Logging;
using AcSaveFormats.ArmoredCoreForAnswer;
using System;

namespace AcSaveConverter.Editors.AcfaEditor.Utilities
{
    internal static class GameProgressValidator
    {
        internal static void Validate(GameProgress data)
        {
            var frsAmount = data.FrsAmount;
            var clamped = Math.Clamp(data.FrsAmount, 0, GameProgress.MaxFrsCount);
            if (frsAmount != clamped)
            {
                Log.WriteLine($"Clamping invalid FRS \"{frsAmount}\" to \"{clamped}\".");
            }

            data.FrsAmount = clamped;
        }
    }
}
