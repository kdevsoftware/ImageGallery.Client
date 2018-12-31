using System;
using FluentValidation.TestHelper;
using ImageGallery.Client.Test.Data;
using ImageGallery.Client.ValidationRules.Gallery;
using Xunit;
using Xunit.Abstractions;

namespace ImageGallery.Client.Test.ValidationRules.Gallery
{
    public class UpdateImageViewModelRuleTest
    {
        private readonly UpdateImageViewModelRule _validator;

        private readonly ITestOutputHelper _output;

        public UpdateImageViewModelRuleTest(ITestOutputHelper output)
        {
            _validator = new UpdateImageViewModelRule();
            _output = output;
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Should_Have_Validation_Error_When_Empty_Properties()
        {
            _validator.ShouldHaveValidationErrorFor(x => x.Id, Guid.Empty);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Should_Pass_Validation_On_ValidModel()
        {
            var data = ImageDataSet.GetValidUpateImage();
            var result = _validator.Validate(data);

            _output.WriteLine(result.IsValid.ToString());
            Assert.True(result.IsValid);
        }
    }
}
