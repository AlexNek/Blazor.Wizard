using Blazor.Wizard.Demo.Models;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Person
{
    public class PersonModelResultBuilder : IWizardResultBuilder<PersonModel>
    {
        public PersonModel Build(IWizardData data)
        {
            if (!data.TryGet<PersonInfoModel>(out var personInfo) || personInfo == null)
            {
                throw new InvalidOperationException("Missing PersonInfoModel data");
            }

            if (!data.TryGet<AddressModel>(out var address) || address == null)
            {
                throw new InvalidOperationException("Missing AddressModel data");
            }

            return new PersonModel
            {
                FirstName = personInfo.FirstName ?? string.Empty,
                LastName = personInfo.LastName ?? string.Empty,
                Email = personInfo.Email ?? string.Empty,
                Street = address.Street ?? string.Empty,
                City = address.City ?? string.Empty,
                ZipCode = address.ZipCode ?? string.Empty,
                Country = address.Country ?? string.Empty,
                Age = personInfo.Age
            };
        }
    }
}