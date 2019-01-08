using FluentValidation;
using ImageGallery.Client.ValidationRules.Constants;
using ImageGallery.Client.ViewModels;

namespace ImageGallery.Client.ValidationRules.Gallery
{
    /// <summary>
    ///
    /// </summary>
    public class AddImageViewModelRule : AbstractValidator<AddImageViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddImageViewModelRule"/> class.
        /// </summary>
        public AddImageViewModelRule()
        {
            RuleFor(x => x.Title).MinimumLength(5).MaximumLength(150).NotEmpty().WithMessage(ImageValidationConstants.TitleEmptyMessage);
            RuleFor(x => x.Category).NotEmpty();
        }
    }
}
