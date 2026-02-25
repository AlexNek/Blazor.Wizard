using System.ComponentModel.DataAnnotations;

namespace Blazor.Wizard.Demo.Models
{
    public class QuestionaryModel
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;
        [Range(1, 120, ErrorMessage = "Age must be greater than 0.")]
        public int Age { get; set; } = 1;
        [Required(ErrorMessage = "Favorite color is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Favorite color is required.")]
        public string FavoriteColor { get; set; } = string.Empty;
    }
}
