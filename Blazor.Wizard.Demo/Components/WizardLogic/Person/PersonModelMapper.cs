using Blazor.Wizard.Demo.Models.Person;
using Blazor.Wizard.Interfaces;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Person;

public class PersonModelMapper : IWizardModelBuilder<PersonModel>, IWizardModelSplitter<PersonModel>
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

    public void Split(PersonModel result, IWizardData data)
    {
        if (!string.IsNullOrWhiteSpace(result.Email) || !string.IsNullOrWhiteSpace(result.FirstName))
        {
            data.Set(new PersonInfoModel
            {
                FirstName = result.FirstName,
                LastName = result.LastName,
                Email = result.Email,
                Age = result.Age
            });
        }

        if (!string.IsNullOrWhiteSpace(result.Street) || !string.IsNullOrWhiteSpace(result.City))
        {
            data.Set(new AddressModel
            {
                Street = result.Street,
                City = result.City,
                ZipCode = result.ZipCode,
                Country = result.Country
            });
        }
    }
}
