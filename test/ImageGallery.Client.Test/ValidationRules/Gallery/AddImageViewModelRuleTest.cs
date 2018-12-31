using FluentValidation.TestHelper;
using ImageGallery.Client.Test.Data;
using ImageGallery.Client.ValidationRules.Gallery;
using Xunit;
using Xunit.Abstractions;

namespace ImageGallery.Client.Test.ValidationRules.Gallery
{
    public class AddImageViewModelRuleTest
    {
        private readonly AddImageViewModelRule _validator;

        private readonly ITestOutputHelper _output;

        public AddImageViewModelRuleTest(ITestOutputHelper output)
        {
            _validator = new AddImageViewModelRule();
            _output = output;
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Should_Have_Validation_Error_When_Empty_Properties()
        {
            _validator.ShouldHaveValidationErrorFor(x => x.Title, (string)null);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Should_Pass_Validation_On_ValidModel()
        {
            var data = ImageDataSet.GetValidAddImage();
            var result = _validator.Validate(data);

            _output.WriteLine(result.IsValid.ToString());
            Assert.True(result.IsValid);
        }
    }
}
