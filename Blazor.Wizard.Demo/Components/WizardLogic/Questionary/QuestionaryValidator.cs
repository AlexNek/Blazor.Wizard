using Blazor.Wizard.Demo.Models;
using System.Collections.Generic;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Questionary
{
    public class QuestionaryValidator : IValidator
    {
        public ValidationResult Validate(object model)
        {
            var errors = new List<string>();
            bool isValid = true;
            // Example validation logic
            if (model is QuestionaryStep1Model step1 && string.IsNullOrWhiteSpace(step1.Name))
            {
                errors.Add("Step1: Name is required.");
                isValid = false;
            }
            if (model is QuestionaryStep2Model step2 && step2.Age < 1)
            {
                errors.Add("Step2: Age must be greater than 0.");
                isValid = false;
            }
            // Add more validation as needed
            return new ValidationResult { IsValid = isValid, Errors = errors };
        }
    }
}
