using System.ComponentModel.DataAnnotations;
using Blazor.Wizard.Interfaces;

namespace Blazor.Wizard.Demo.Models.Person;

public class PersonInfoModel : IWizardDataModel
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

    [Range(6, 120)]
    public int Age { get; set; }    
}