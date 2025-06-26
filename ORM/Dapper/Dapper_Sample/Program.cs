using Dapper_Sample.Models;
using Dapper_Sample.Models.Shared;
using Dapper_Sample.Shared;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", async () =>
{
    try
    {
        await DataHelper.InsertContracts();
        await DataHelper.InsertCustomer();
        await DataHelper.AddContractsToCustomers();
        var targetCustomer = new Customer()
        {
            FirstName = "John",
            LastName = "Doe",
        };
        var mobileContracts = await DataHelper.GetContractsByType(ContractType.Mobile);
        var dbCustomer = await DataHelper.GetCustomer(targetCustomer);

        await DataHelper.TPH_ContractsQuery();
        await DataHelper.CustomerMobileContractNavigation();
        await DataHelper.CustomerContractNavigation();
        await DataHelper.CleanUpSample();

    }
    catch (Exception ex)
    {
        await DataHelper.CleanUpSample();
        throw;
    }

    return "Hello World!";
});

app.Run();