using FluentValidation;
using ImageGallery.Client.ViewModels.UserManagement;

namespace ImageGallery.Client.ValidationRules
{
    /// <summary>
    ///  User Profile Validation Rule.
    /// </summary>
    public class UserProfileRule : AbstractValidator<UserProfileUpdateViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileRule"/> class.
        /// </summary>
        public UserProfileRule()
        {
            RuleFor(m => m.FirstName).NotEmpty();

            RuleFor(m => m.LastName).NotEmpty();

            RuleFor(m => m.Address).NotEmpty();
        }
    }
}
