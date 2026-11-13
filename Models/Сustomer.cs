namespace AdapterImport.Core.Models;

public enum CustomerStatus
{
    Unknown = 0,
    Active = 1,
    Suspended = 2,
    Archived = 9
}

// TODO 2: Создать современную модель данных.
// Используем неизменяемый record, строгие типы (DateOnly, decimal, Guid).
public sealed record Customer(
    Guid Id,
    string ExternalId,
    string FullName,
    string Email,
    DateOnly BirthDate,
    CustomerStatus Status,
    decimal Salary,
    int SourceLineNumber);