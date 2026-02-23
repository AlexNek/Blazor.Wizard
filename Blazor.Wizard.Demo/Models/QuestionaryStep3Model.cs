using System.ComponentModel.DataAnnotations;

namespace Blazor.Wizard.Demo.Models
{
    public class QuestionaryStep3Model
    {
        [Required(ErrorMessage = "Favorite color is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Favorite color is required.")]
        public string FavoriteColor { get; set; } = string.Empty;
    }
}
