using System.Linq;
using FluentAssertions;
using FluentValidation.TestHelper;
using ImageGallery.Client.Test.Data;
using ImageGallery.Client.ValidationRules;
using ImageGallery.Client.ValidationRules.Constants;
using ImageGallery.Model.Models.Albums;
using Xunit;
using Xunit.Abstractions;

namespace ImageGallery.Client.Test.ValidationRules
{
    public class AlbumProperitesUpdateRuleTest
    {
        private readonly AlbumProperitesUpdateRule _validator;

        private readonly ITestOutputHelper _output;

        public AlbumProperitesUpdateRuleTest(ITestOutputHelper output)
        {
            _validator = new AlbumProperitesUpdateRule();
            _output = output;
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Should_Have_Validation_Error_When_Empty_Properties()
        {
            _validator.ShouldHaveValidationErrorFor(x => x.Title, (string)null);
            _validator.ShouldHaveValidationErrorFor(x => x.Description, (string)null);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Should_Have_Validation_Errors_When_InValid_Property_Is_Supplied()
        {
            var item = new AlbumProperitesUpdate
            {
                Title = null,
                Description = null,
            };

            var titleValidationResult = _validator.ValidatePropertyAndReturnErrors(item, "Title");
            var descriptionValidationResult = _validator.ValidatePropertyAndReturnErrors(item, "Description");

            var titleValidationMessage = titleValidationResult.FirstOrDefault();
            var descriptionValidationMessage = descriptionValidationResult.FirstOrDefault();

            _output.WriteLine(titleValidationMessage);
            _output.WriteLine(descriptionValidationMessage);

            Assert.Equal(AlbumValidationConstants.TitleEmptyMessage, titleValidationMessage);
            Assert.Equal(AlbumValidationConstants.DescriptionEmptyMessage, descriptionValidationMessage);

            titleValidationMessage.Should().BeEquivalentTo(AlbumValidationConstants.TitleEmptyMessage);
            descriptionValidationMessage.Should().BeEquivalentTo(AlbumValidationConstants.DescriptionEmptyMessage);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Should_Pass_Validation_On_ValidModel()
        {
            var data = AlbumDataSet.GetAlbumProperitesUpdateModel();
            var result = _validator.Validate(data);

            _output.WriteLine(result.IsValid.ToString());
            Assert.True(result.IsValid);
        }
    }
}
