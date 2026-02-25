using Blazor.Wizard.Demo.Models;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Questionary
{
    public class QuestionaryValidator : IValidator
    {
        public ValidationResult Validate(object model)
        {
            // Custom business validation only
            // DataAnnotations are handled by EditContext.Validate()
            return new ValidationResult { IsValid = true, Errors = new List<string>() };
        }
    }
}
