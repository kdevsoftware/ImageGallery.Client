using FluentValidation.TestHelper;
using ImageGallery.Client.Test.Data;
using ImageGallery.Client.ValidationRules.Gallery;
using Xunit;
using Xunit.Abstractions;

namespace ImageGallery.Client.Test.ValidationRules
{
    public class EditImageViewModelRuleTest
    {
        private readonly EditImageViewModelRule _validator;

        private readonly ITestOutputHelper _output;

        public EditImageViewModelRuleTest(ITestOutputHelper output)
        {
            _validator = new EditImageViewModelRule();
            _output = output;
        }

        [Fact]
        public void Should_Have_Validation_Error_When_Empty_Properties()
        {
            _validator.ShouldHaveValidationErrorFor(x => x.Title, (string)null);
        }

        [Fact]
        public void Should_Pass_Validation_On_ValidModel()
        {
            var data = ImageDataSet.GetValidEditImage();
            var result = _validator.Validate(data);

            _output.WriteLine(result.IsValid.ToString());
            Assert.True(result.IsValid);
        }
    }
}
