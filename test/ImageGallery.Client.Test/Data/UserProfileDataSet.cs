using System.Collections.Generic;
using System.Linq;
using Bogus;
using ImageGallery.Client.ViewModels.UserManagement;

namespace ImageGallery.Client.Test.Data
{
    public static class UserProfileDataSet
    {
        public static UserProfileViewModel GetUserProfile()
        {
            return GetUserProfileTableData(1).Select(x => x).First();
        }

        public static List<UserProfileViewModel> GetUserProfileTableData(int count)
        {
            var userProfileFaker = new Faker<UserProfileViewModel>()
                .RuleFor(c => c.FirstName, f => f.Name.FirstName())
                .RuleFor(c => c.LastName, f => f.Name.LastName())
                .RuleFor(c => c.Address, f => f.Address.StreetAddress())
                .RuleFor(c => c.Address2, f => f.Address.SecondaryAddress())
                .RuleFor(c => c.City, f => f.Address.City())
                .RuleFor(c => c.State, f => f.Address.StateAbbr())
                .RuleFor(c => c.PostalCode, f => f.Address.ZipCode())
                .RuleFor(c => c.Country, f => f.Address.CountryCode())
                .RuleFor(c => c.Language, f => f.Name.Random.AlphaNumeric(10));

            var userProfile = userProfileFaker.Generate(count);
            return userProfile;
        }
    }
}

