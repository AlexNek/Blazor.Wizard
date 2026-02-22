using Blazor.Wizard.Demo.Models;

namespace Blazor.Wizard.Demo.Components.Wizard
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

            // Basic validation example
            if (string.IsNullOrWhiteSpace(personInfo.FirstName) || string.IsNullOrWhiteSpace(personInfo.LastName))
            {
                throw new InvalidOperationException("PersonInfoModel is incomplete");
            }

            if (string.IsNullOrWhiteSpace(address.Street) || string.IsNullOrWhiteSpace(address.City))
            {
                throw new InvalidOperationException("AddressModel is incomplete");
            }

            return new PersonModel
            {
                FirstName = personInfo.FirstName,
                LastName = personInfo.LastName,
                Email = personInfo.Email,
                Street = address.Street,
                City = address.City,
                ZipCode = address.ZipCode,
                Country = address.Country,
                Age = personInfo.Age
            };
        }
    }
}