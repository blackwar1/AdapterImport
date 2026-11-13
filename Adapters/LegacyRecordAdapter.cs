using System.Globalization;
using AdapterImport.Core.Abstractions;
using AdapterImport.Core.Models;

namespace AdapterImport.Core.Adapters;

// TODO 4: Создать Adapter для старого формата.
public sealed class LegacyRecordAdapter : IRecordAdapter
{
    public Customer Adapt(LegacyRecord record)
    {
        var parts = record.RawLine.Split('|');
        if (parts.Length != 6)
            throw new FormatException($"Ожидалось 6 полей, получено {parts.Length}.");

        if (string.IsNullOrWhiteSpace(parts[0]))
            throw new FormatException("Пустой ID.");

        if (!DateOnly.TryParseExact(parts[3].Trim(), "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var birth))
            throw new FormatException($"Некорректная дата: '{parts[3]}'.");

        if (!int.TryParse(parts[4].Trim(), out var statusCode) || !Enum.IsDefined(typeof(CustomerStatus), statusCode))
            throw new FormatException($"Некорректный статус: '{parts[4]}'.");

        if (!decimal.TryParse(parts[5].Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var salary))
            throw new FormatException($"Некорректная зарплата: '{parts[5]}'.");

        return new Customer(
            Id: Guid.NewGuid(),
            ExternalId: parts[0].Trim(),
            FullName: parts[1].Trim(),
            Email: parts[2].Trim(),
            BirthDate: birth,
            Status: (CustomerStatus)statusCode,
            Salary: salary,
            SourceLineNumber: record.LineNumber);
    }
}