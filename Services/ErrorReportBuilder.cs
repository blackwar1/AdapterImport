using System.Text;
using AdapterImport.Core.Models;

namespace AdapterImport.Core.Services;

public static class ErrorReportBuilder
{
    // TODO 9: Сформировать отчёт ошибок через LINQ.
    public static string Build(ImportResult result)
    {
        if (result.Errors.Count == 0) return "Ошибок нет.";

        var grouped = result.Errors
            .GroupBy(e => e.Category)
            .Select(g => new { Category = g.Key, Count = g.Count(), Example = g.First().Reason })
            .OrderByDescending(x => x.Count);

        var sb = new StringBuilder();
        sb.AppendLine($"Отчёт (всего ошибок: {result.Errors.Count}):");
        foreach (var g in grouped)
            sb.AppendLine($"  [{g.Category}] - {g.Count} шт. Пример: {g.Example}");

        return sb.ToString();
    }
}