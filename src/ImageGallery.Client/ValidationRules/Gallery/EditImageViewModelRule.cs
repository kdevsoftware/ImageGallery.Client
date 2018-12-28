using FluentValidation;
using ImageGallery.Client.ValidationRules.Constants;
using ImageGallery.Client.ViewModels;

namespace ImageGallery.Client.ValidationRules.Gallery
{
    /// <summary>
    ///  Edit Image Validation Rule.
    /// </summary>
    public class EditImageViewModelRule : AbstractValidator<EditImageViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditImageViewModelRule"/> class.
        /// </summary>
        public EditImageViewModelRule()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Title).NotEmpty().WithMessage(ImageValidationConstants.TitleEmptyMessage);
            RuleFor(x => x.Category).NotEmpty();
        }
    }
}
