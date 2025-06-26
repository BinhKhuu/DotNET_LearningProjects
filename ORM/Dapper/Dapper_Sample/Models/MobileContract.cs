using Dapper_Sample.Models.Shared;

namespace Dapper_Sample.Models;

public class MobileContract : Contract
{
    public MobileContract() => ContractType = ContractType.Mobile;
    public string MobileNumber { get; set; }
}
