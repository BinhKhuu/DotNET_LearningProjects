using System.ComponentModel.DataAnnotations;

namespace Dapper_Sample.Models;

public class Customer
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string FirstName { get; set; }

    [Required, MaxLength(100)]
    public string LastName { get; set; }

    [Required, EmailAddress, MaxLength(255)]
    public string Email { get; set; }

    [Phone, MaxLength(20)]
    public string PhoneNumber { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property if you want to associate contracts
    public List<Contract> Contracts { get; set; } = new List<Contract>();
}
