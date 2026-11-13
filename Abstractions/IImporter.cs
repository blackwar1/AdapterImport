using AdapterImport.Core.Models;

namespace AdapterImport.Core.Abstractions;

// TODO 3: Реализовать интерфейс импортёра.
public interface IImporter
{
    Task<ImportResult> ImportAsync(string filePath, CancellationToken ct = default);
}