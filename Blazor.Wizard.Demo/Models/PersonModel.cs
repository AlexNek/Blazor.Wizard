using System.ComponentModel.DataAnnotations;
namespace Blazor.Wizard.Demo.Models;

public class PersonModel
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 5)]
    public string Street { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string City { get; set; } = string.Empty;

    [Required]
    [StringLength(10, MinimumLength = 2)]
    public string ZipCode { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Country { get; set; } = string.Empty;

    [Range(6, 120)]
    public int Age { get; set; }
}