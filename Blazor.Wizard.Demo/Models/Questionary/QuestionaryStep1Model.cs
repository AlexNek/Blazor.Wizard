using System.ComponentModel.DataAnnotations;

namespace Blazor.Wizard.Demo.Models.Questionary
{
    public class QuestionaryStep1Model
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;
    }
}
