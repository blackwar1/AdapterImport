namespace AdapterImport.Core.Models;

public sealed record ImportError(
    int LineNumber,
    string RawLine,
    string Reason,
    string Category);