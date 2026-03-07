using System.ComponentModel.DataAnnotations;

namespace Blazor.Wizard.Demo.Models.Questionary
{
    public class QuestionaryStep2Model
    {
        [Range(1, 120, ErrorMessage = "Age must be greater than 0.")]
        public int Age { get; set; }
    }
}
