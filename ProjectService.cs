using AdapterImport.Core.Adapters;
using AdapterImport.Core.Models;
using AdapterImport.Core.Services;
using AdapterImport.Core.Settings;

namespace AdapterImport;

public sealed class ProjectService
{
    public async Task RunAsync()
    {
        // TODO 7: Вынести импортёр и модели в DLL.
        // Выполнено: Модели, адаптеры и сервисы вынесены в проект AdapterImport.Core (DLL).
        // Здесь (в консольном проекте) остались только точка входа и сборка зависимостей.

        var settings = RegistryImportSettings.Load();
        Console.WriteLine($"Настройки: MaxParallelism={settings.MaxParallelism}");

        var importer = new LegacyFileImporter(new LegacyRecordAdapter(), settings.MaxParallelism);

        var samplePath = Path.Combine(AppContext.BaseDirectory, "input.txt");
        EnsureSampleFile(samplePath);

        Console.WriteLine($"Импорт файла: {samplePath}");
        ImportResult result;
        try
        {
            result = await importer.ImportAsync(samplePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Критическая ошибка: {ex.Message}");
            return;
        }

        Console.WriteLine($"Успешно: {result.Customers.Count} из {result.TotalRead} ({result.SuccessRate:P0})");
        Console.WriteLine(ErrorReportBuilder.Build(result));

        RegistryImportSettings.Save(settings);
    }

    private static void EnsureSampleFile(string path)
    {
        if (File.Exists(path)) return;
        File.WriteAllLines(path, new[]
        {
            "A001|Иван Петров|ivan@example.com|15031990|1|50000.50",
            "A002|Мария Сидорова|maria@example.com|22071985|2|61000",
            "A003|Плохая запись|no-email|01012000|1|1000",
            "A004|Олег Смирнов|oleg@example.com|99999999|1|70000",
            "A005|Пустой статус|a5@example.com|01011995|X|50000",
            "A006|Дубль Id|dup@example.com|01011980|1|1",
            "A006|Дубль Id 2|dup2@example.com|02021981|1|2",
            "A007|Без полей|только одно поле"
        });
    }
}