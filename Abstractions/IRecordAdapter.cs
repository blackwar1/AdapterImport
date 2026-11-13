using AdapterImport.Core.Models;

namespace AdapterImport.Core.Abstractions;

public interface IRecordAdapter
{
    Customer Adapt(LegacyRecord record);
}