using Dapper_Sample.Models;
using Dapper_Sample.Models.Shared;
using Dapper;
using Npgsql;

namespace Dapper_Sample.Shared;

public static class DataHelper
{
    private readonly static string connectionString = "Host=localhost;Port=5432;Database=Dapper_Sample";

    public static async Task InsertContracts()
    {
        var mobileContract1 = new MobileContract()
        {
            StartDate = default(DateTime),
            DurationMonths = 2,
            Charge = 20,
            CustomerId = 1,
            MobileNumber = "12345678",
        };

        var TvContract1 = new TvContract()
        {
            StartDate = default(DateTime),
            DurationMonths = 2,
            Charge = 20,
            CustomerId = 1,
            TVPackageType = TVPackageType.L
        };
        
        await using var connection = new NpgsqlConnection(connectionString);
        var insertMobileQuery = @"INSERT INTO contracts (StartDate, DurationMonths, Charge, CustomerId,MobileNumber, ContractType)
                    Values (@StartDate, @DurationMonths, @Charge, @CustomerId, @MobileNumber, @ContractType);";
        var insertTVQuery = @"INSERT INTO contracts (StartDate, DurationMonths, Charge, CustomerId,TVPackageType, ContractType)
                    Values (@StartDate, @DurationMonths, @Charge, @CustomerId, @TVPackageType, @ContractType);";
        await connection.ExecuteAsync(
            insertMobileQuery,
            mobileContract1);
        await connection.ExecuteAsync(
            insertTVQuery,
            TvContract1);
    }

    public static async Task<List<MobileContract>> GetContractsByType(ContractType contractType)
    {
        var query = "select * from contracts where ContractType=@ContractType";
        await using var connection = new NpgsqlConnection(connectionString);
        var contracts = await connection.QueryAsync<MobileContract>(
            query,
            new {ContractType = contractType});
        return contracts.ToList();
    }

    public static async Task DeleteContracts()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        var deleteQuery = "DELETE FROM contracts; ALTER SEQUENCE  contracts_id_seq RESTART WITH 1;";
        await connection.ExecuteAsync(deleteQuery);
    }

    public static async Task AddContractsToCustomers()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        var mobileContract1 = await GetContractsByType(ContractType.Mobile);
        var tvContract1 = await GetContractsByType(ContractType.TV);
        
        if(mobileContract1.Count == 0)
            throw new Exception("There are no mobile contracts");
        if(tvContract1.Count == 0)
            throw new Exception("There are no TV contracts");
        
        var customers = await GetCustomers();
        if(customers.Count == 0)
            throw new Exception("There are no customers");

        // just doing the first
        foreach (var mobileContract in mobileContract1)
        {
            mobileContract.CustomerId = customers[0].Id;
        }
        
        // just doing the first
        foreach (var tvContract in tvContract1)
        {
            tvContract.CustomerId = customers[0].Id;
        }
        
        var updateQuery = "Update contracts set CustomerId = @CustomerId Where Id = @Id ";
        await connection.ExecuteAsync(updateQuery, new
        {
            CustomerId = mobileContract1[0].CustomerId,
            Id = mobileContract1[0].Id,
        });
        
        await connection.ExecuteAsync(updateQuery, new
        {
            CustomerId = tvContract1[0].CustomerId,
            Id = tvContract1[0].Id,
        });
        
    }
    public static async Task InsertCustomer()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        var insertQuery = @"
                INSERT INTO Customers (FirstName, LastName, Email, DateOfBirth) 
                VALUES (@FirstName, @LastName, @Email, @DateOfBirth);";
        var newMobileCustomer = new Customer()
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@mail.com",
            DateOfBirth = DateTime.Now
        };
        await connection.ExecuteAsync(
            insertQuery,
            newMobileCustomer);
    }

    public static async Task<List<Customer>> GetCustomers()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        var query = "select * from Customers";
        var customers = await connection.QueryAsync<Customer>(query);
        return customers.ToList();
    }
    public static async Task<Customer> GetCustomer(Customer customer)
    {
        var (sql, parameters) = BuildCustomerFilter(customer);
        await using var connection = new NpgsqlConnection(connectionString);
        return await connection.QueryFirstOrDefaultAsync<Customer>(sql, parameters);
    }
    
    public static (string Sql, DynamicParameters Params) BuildCustomerFilter(Customer filter)
    {
        var whereClauses = new List<string>();
        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(filter.FirstName))
        {
            whereClauses.Add("FirstName = @FirstName");
            parameters.Add("FirstName", filter.FirstName);
        }

        if (!string.IsNullOrEmpty(filter.LastName))
        {
            whereClauses.Add("LastName = @LastName");
            parameters.Add("LastName", filter.LastName);
        }

        if (!string.IsNullOrEmpty(filter.Email))
        {
            whereClauses.Add("Email = @Email");
            parameters.Add("Email", filter.Email);
        }

        if (filter.DateOfBirth.HasValue)
        {
            whereClauses.Add("DateOfBirth = @DateOfBirth");
            parameters.Add("DateOfBirth", filter.DateOfBirth.Value);
        }

        var sql = "SELECT * FROM customers";
        if (whereClauses.Any())
        {
            sql += " WHERE " + string.Join(" AND ", whereClauses);
        }

        return (sql, parameters);
    }
    
    public static async Task DeleteCustomers()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        var deleteQuery = "delete from customers; ALTER SEQUENCE customers_id_seq RESTART WITH 1;";
        await connection.ExecuteAsync(deleteQuery);
    }

    public static async Task CleanUpSample()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await DeleteCustomers();
        await DeleteContracts();
    }

    // TPH Inheritance https://www.learndapper.com/hierarchical-data
    public static async Task<(List<TvContract> TvContracts, List<MobileContract> MobileContracts)> TPH_ContractsQuery()
    {
        var tvContracts = new List<TvContract>();
        var mobileContracts = new List<MobileContract>();
        await using var connection = new NpgsqlConnection(connectionString);
        
        var query = "select * from contracts";
        using var reader = connection.ExecuteReader(query);
        var tvContractsParse = reader.GetRowParser<TvContract>();
        var mobileContractsParse = reader.GetRowParser<MobileContract>();

        while (reader.Read())
        {
            //find the column position for ContractType in table Contracts and get is value which should be a value present in the ContractType Enum
            var discriminator = (ContractType)reader.GetInt32(reader.GetOrdinal(nameof(Contract.ContractType)));
            switch (discriminator)
            {
                case ContractType.TV:
                    tvContracts.Add(tvContractsParse(reader));
                    break;
                case ContractType.Mobile:
                    mobileContracts.Add(mobileContractsParse(reader));
                    break;
            }
        }
        // Mobile Contracts
        Console.WriteLine("Mobile Contracts");
        mobileContracts.ForEach(c => Console.WriteLine($"Duration: {c.DurationMonths} months, Number: {c.MobileNumber}"));
        // TV Contracts
        Console.WriteLine("TV Contracts");
        tvContracts.ForEach(c => Console.WriteLine($"Duration: {c.DurationMonths} months, Package Type: {c.TVPackageType.ToString()}"));
        
        return (tvContracts, mobileContracts);
    }

    public static async Task CustomerContractNavigation()
    {
        var (tvContracts, mobileContracts) = await TPH_ContractsQuery();
        await using var connection = new NpgsqlConnection(connectionString);
        var query = "select * from customers";
        var customers = await connection.QueryAsync<Customer>(query);
        var customerDict = customers.ToDictionary(customer => customer.Id, customer => customer);
        foreach (var mobileContract in mobileContracts)
        {
            customerDict.TryGetValue(mobileContract.Id, out var customer);
            customer?.Contracts.Add(mobileContract);
        }
        
        foreach (var tvContract in tvContracts)
        {
            customerDict.TryGetValue(tvContract.Id, out var customer);
            customer?.Contracts.Add(tvContract);
        }

        foreach (var kvp in customerDict)
        {
            Console.WriteLine($"Key: {kvp.Key}, Customer Name: {kvp.Value.FirstName} {kvp.Value.LastName}");
            foreach (var customerContracts in kvp.Value.Contracts)
            {
                Console.WriteLine($"Contract: {customerContracts.ContractType}");
            }
        }
    }
    
    public static async Task CustomerMobileContractNavigation()
    {
        
        await using var connection = new NpgsqlConnection(connectionString);
        var query = @"
            Select cust.*, con.* from Customers cust
            Inner Join Contracts con on con.CustomerId = cust.Id
            where con.ContractType = 1
            ";
        
        var customers = await connection.QueryAsync<Customer,MobileContract, Customer>(
            query,
            (customer, contract) =>
            {
                // Customer.Contracts = new List<Contract>();
                customer.Contracts.Add(contract);
                return customer;
            },
            splitOn: "Id");

        var groupedCustomers = customers.GroupBy(customer => customer.Id)
            .Select(g =>
            {
                var groupedCustomer = g.First(); //first customer as base
                groupedCustomer.Contracts = g.Select(c => c.Contracts.Single()).ToList(); // add all contracts to base
                return groupedCustomer;
            });

        foreach (var customer in groupedCustomers)
        {
            Console.WriteLine($"Customer: {customer.FirstName} {customer.LastName}");
            foreach (var contract in customer.Contracts)
            {
                Console.WriteLine($"Contract: {contract.ContractType}");
            }
        }
    }
    
}