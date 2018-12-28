using FluentValidation;
using ImageGallery.Client.ValidationRules.Constants;
using ImageGallery.Model.Models.Albums;

namespace ImageGallery.Client.ValidationRules
{
    /// <summary>
    /// Album Properites Update Validation Rule.
    /// </summary>
    public class AlbumProperitesUpdateRule : AbstractValidator<AlbumProperitesUpdate>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumProperitesUpdateRule"/> class.
        /// </summary>
        public AlbumProperitesUpdateRule()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage(AlbumValidationConstants.TitleEmptyMessage);
            RuleFor(x => x.Description).NotEmpty().WithMessage(AlbumValidationConstants.DescriptionEmptyMessage);
        }
    }
}
