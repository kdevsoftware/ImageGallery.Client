using System;
using FluentValidation;
using ImageGallery.Client.ViewModels;

namespace ImageGallery.Client.ValidationRules.Gallery
{
    /// <summary>
    ///
    /// </summary>
    public class UpdateImageViewModelRule : AbstractValidator<UpdateImageViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateImageViewModelRule"/> class.
        /// </summary>
        public UpdateImageViewModelRule()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
