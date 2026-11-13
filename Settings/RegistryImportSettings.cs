using Microsoft.Win32;

namespace AdapterImport.Core.Settings;

public sealed record ImportSettings(int MaxParallelism);

public static class RegistryImportSettings
{
    private const string KeyPath = @"Software\Variant18\Import";

    // TODO 10: Сохранить настройки импорта в реестре.
    public static ImportSettings Load()
    {
        if (!OperatingSystem.IsWindows()) return new ImportSettings(4);
        using var key = Registry.CurrentUser.OpenSubKey(KeyPath);
        return new ImportSettings(key?.GetValue("MaxParallelism") as int? ?? 4);
    }

    public static void Save(ImportSettings settings)
    {
        if (!OperatingSystem.IsWindows()) return;
        using var key = Registry.CurrentUser.CreateSubKey(KeyPath);
        key.SetValue("MaxParallelism", settings.MaxParallelism, RegistryValueKind.DWord);
    }
}