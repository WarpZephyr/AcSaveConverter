using AcSaveFormats.ArmoredCoreForAnswer;

namespace AcSaveConverter.Editors.AcfaEditor.Data
{
    public record ExportGameData(
        Design Design,
        GameProgress GameProgress,
        OptionsSettings OptionsSettings,
        PlayerData PlayerData);
}
