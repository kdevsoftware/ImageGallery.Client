using FluentValidation;
using ImageGallery.Client.ValidationRules.Constants;
using ImageGallery.Model.Models.Images;

namespace ImageGallery.Client.ValidationRules.Gallery
{
    /// <summary>
    ///  Image Properites Update Validation Rule.
    /// </summary>
    public class ImageProperitesUpdateRule : AbstractValidator<ImageProperitesUpdate>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageProperitesUpdateRule"/> class.
        /// </summary>
        public ImageProperitesUpdateRule()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage(ImageValidationConstants.TitleEmptyMessage);
        }
    }
}
