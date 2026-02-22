using System.ComponentModel.DataAnnotations;

namespace Blazor.Wizard.DemoDevEx.Models;

public class AddressModel
{
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

}
