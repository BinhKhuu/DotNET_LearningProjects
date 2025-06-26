using Dapper_Sample.Models.Shared;

namespace Dapper_Sample.Models;

public class TvContract : Contract
{
    public TvContract() => ContractType = ContractType.TV;
    public TVPackageType TVPackageType { get; set; }
}
