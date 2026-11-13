namespace AdapterImport.Core.Models;

public sealed record ImportResult(
    IReadOnlyList<Customer> Customers,
    IReadOnlyList<ImportError> Errors,
    TimeSpan Elapsed)
{
    public int TotalRead => Customers.Count + Errors.Count;
    public double SuccessRate => TotalRead == 0 ? 0 : (double)Customers.Count / TotalRead;
}