using System.Linq;
using FluentAssertions;
using FluentValidation.TestHelper;
using ImageGallery.Client.Test.Data;
using ImageGallery.Client.ValidationRules;
using ImageGallery.Client.ValidationRules.Constants;
using ImageGallery.Client.ValidationRules.Gallery;
using ImageGallery.Model.Models.Images;
using Xunit;
using Xunit.Abstractions;

namespace ImageGallery.Client.Test.ValidationRules.Gallery
{
    public class ImageProperitesUpdateRuleTest
    {
        private readonly ImageProperitesUpdateRule _validator;

        private readonly ITestOutputHelper _output;

        public ImageProperitesUpdateRuleTest(ITestOutputHelper output)
        {
            _validator = new ImageProperitesUpdateRule();
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
        public void Should_Have_Validation_Errors_When_InValid_Property_Is_Supplied()
        {
            var item = new ImageProperitesUpdate
            {
                Title = null,
            };

            var titleValidationResult = _validator.ValidatePropertyAndReturnErrors(item, "Title");
            var titleValidationMessage = titleValidationResult.FirstOrDefault();

            _output.WriteLine(titleValidationMessage);

            Assert.Equal(AlbumValidationConstants.TitleEmptyMessage, titleValidationMessage);
            titleValidationMessage.Should().BeEquivalentTo(ImageValidationConstants.TitleEmptyMessage);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Should_Pass_Validation_On_ValidModel()
        {
            var data = ImageDataSet.GetValidImageProperitesUpdate();
            var result = _validator.Validate(data);

            _output.WriteLine(result.IsValid.ToString());
            Assert.True(result.IsValid);
        }
    }
}
