using Dapper_Sample.Models.Shared;

namespace Dapper_Sample.Models;

public abstract class Contract
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public int DurationMonths { get; set;}
    public decimal Charge { get; set; }
    public ContractType ContractType { get; set; }
    public int CustomerId { get; set; }
    
    // navigation property
    public List<Customer> Customers { get; set; } = new List<Customer>();
}
