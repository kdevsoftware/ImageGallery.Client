using FluentAssertions;
using FluentValidation.TestHelper;
using ImageGallery.Client.ValidationRules;
using ImageGallery.Client.ViewModels.UserManagement;
using Xunit;

namespace ImageGallery.Client.Test.ValidationRules
{
    public class UserProfileRuleTest
    {
        private readonly UserProfileRule _validator;

        public UserProfileRuleTest()
        {
            _validator = new UserProfileRule();
        }

        [Fact]
        public void Should_Have_Validation_Error_When_Empty_Properties()
        {
            _validator.ShouldHaveValidationErrorFor(x => x.FirstName, (string)null);
        }

        [Fact]
        public void Should_Have_Valid_Properties()
        {
            _validator.ShouldNotHaveValidationErrorFor(x => x.FirstName, "testfirstname");
        }

        [Fact]
        public void Should_Not_Have_Validation_Errors_When_ValidModel_Is_Supplied()
        {
            var item = new UserProfileUpdateViewModel
            {
                FirstName = "firstName",
                LastName = "lastName",
                Address = "sample address 1",
            };

            var validationResult = _validator.Validate(item);
            validationResult.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Should_Not_Have_Validation_Errors_When_ValidProperty_Is_Supplied()
        {
            var item = new UserProfileUpdateViewModel
            {
                FirstName = "firstName",
                LastName = "lastName",
                Address = "sample address 1",
            };

            var validationFirstNameResult = _validator.ValidateProperty(item, "FirstName");
            validationFirstNameResult.IsValid.Should().BeTrue();

            var validationLastNameResult = _validator.ValidateProperty(item, "LastName");
            validationLastNameResult.IsValid.Should().BeTrue();

            var validationAddressResult = _validator.ValidateProperty(item, "Address");
            validationAddressResult.IsValid.Should().BeTrue();

            var validationGroupResult = _validator.ValidateProperties(item, new[] { "FirstName", "LastName" });
            validationGroupResult.IsValid.Should().BeTrue();
        }
    }
}
