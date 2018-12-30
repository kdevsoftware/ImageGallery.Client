using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ImageGallery.Client.Configuration;
using ImageGallery.Client.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ImageGallery.Client.Apis.Base
{
    /// <summary>
    ///
    /// </summary>
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        ///
        /// </summary>
        protected ApplicationOptions ApplicationSettings { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        protected async Task WriteOutIdentityInformation()
        {
            // get the saved identity token
            var identityToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);

            // write it out
            Debug.WriteLine($"Identity token: {identityToken}");

            // write out the user claims
            foreach (var claim in User.Claims)
            {
                Debug.WriteLine($"Claim type: {claim.Type} - Claim value: {claim.Value}");
            }
        }

        /// <summary>
        /// Generic Reflection Patch Name/Value.
        /// Basic Use for Flat Objects.
        /// </summary>
        /// <typeparam name="TEntity">Entity.</typeparam>
        /// <param name="entity"></param>
        /// <param name="patchDtos"></param>
        /// <returns></returns>
        protected TEntity ApplyPatch<TEntity>(TEntity entity, List<PatchDto> patchDtos)
            where TEntity : class
        {
            // TODO: Handle Invalid Properties
            var nameValuePairProperties = patchDtos.ToDictionary(a => a.PropertyName, a => a.PropertyValue);
            foreach (var property in nameValuePairProperties)
            {
                PropertyInfo propertyInfo = entity.GetType().GetProperty(property.Key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                propertyInfo.SetValue(entity, Convert.ChangeType(property.Value, propertyInfo.PropertyType), null);
            }

            return entity;
        }
    }
}
