using System.Diagnostics;
using AdapterImport.Core.Abstractions;
using AdapterImport.Core.Models;

namespace AdapterImport.Core.Services;

public sealed class LegacyFileImporter : IImporter
{
    private readonly IRecordAdapter _adapter;
    private readonly int _maxParallelism;

    public LegacyFileImporter(IRecordAdapter adapter, int maxParallelism = 4)
    {
        _adapter = adapter;
        _maxParallelism = Math.Max(1, maxParallelism);
    }

    public async Task<ImportResult> ImportAsync(string filePath, CancellationToken ct = default)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Файл не найден", filePath);

        var sw = Stopwatch.StartNew();
        var customers = new List<Customer>();
        var errors = new List<ImportError>();
        var lineNo = 0;

        // TODO 5: Асинхронно читать входной файл.
        await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
        using var reader = new StreamReader(stream, detectEncodingFromByteOrderMarks: true);

        while (await reader.ReadLineAsync(ct) is { } line)
        {
            lineNo++;
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#')) continue;

            // TODO 6: Обрабатывать некорректные строки отдельно, не прекращая импорт.
            try
            {
                customers.Add(_adapter.Adapt(new LegacyRecord(lineNo, line)));
            }
            catch (FormatException ex)
            {
                errors.Add(new ImportError(lineNo, line, ex.Message, "format"));
            }
        }

        // TODO 8: Добавить параллельную валидацию независимых записей.
        await ValidateParallelAsync(customers, errors, ct);

        sw.Stop();
        return new ImportResult(customers, errors, sw.Elapsed);
    }

    private Task ValidateParallelAsync(List<Customer> customers, List<ImportError> errors, CancellationToken ct)
    {
        return Task.Run(() =>
        {
            var duplicates = customers.AsParallel().WithCancellation(ct)
                .GroupBy(c => c.ExternalId).Where(g => g.Count() > 1).Select(g => g.Key).ToHashSet();

            if (duplicates.Count == 0) return;

            lock (errors)
            {
                foreach (var c in customers.Where(c => duplicates.Contains(c.ExternalId)))
                    errors.Add(new ImportError(c.SourceLineNumber, c.ExternalId, $"Дубликат ID: {c.ExternalId}", "duplicate"));
            }
        }, ct);
    }
}