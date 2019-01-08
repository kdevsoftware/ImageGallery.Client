using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;

namespace ImageGallery.Client.ValidationRules
{
    /// <summary>
    ///  Fluent Validation Extensions.
    /// </summary>
    public static class ValidationExtensions
    {
        /// <summary>
        ///  Validate Property.
        /// </summary>
        /// <param name="validator"></param>
        /// <param name="target"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static ValidationResult ValidateProperty(this IValidator validator, object target, string propertyName)
        {
            var context = new ValidationContext(target, new PropertyChain(), new MemberNameValidatorSelector(new[] { propertyName }));
            return validator.Validate(context);
        }

        /// <summary>
        ///  Validate Single Properties.
        /// </summary>
        /// <param name="validator"></param>
        /// <param name="target"></param>
        /// <param name="propertyNames"></param>
        /// <returns></returns>
        public static ValidationResult ValidateProperties(this IValidator validator, object target, string[] propertyNames)
        {
            var context = new ValidationContext(target, new PropertyChain(), new MemberNameValidatorSelector(propertyNames));

            return validator.Validate(context);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="validator"></param>
        /// <param name="target"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static IEnumerable<string> ValidatePropertyAndReturnErrors(this IValidator validator, object target, string propertyName)
        {
            var result = validator.ValidateProperty(target, propertyName);
            return result.Errors.Select(x => x.ErrorMessage);
        }
    }
}
