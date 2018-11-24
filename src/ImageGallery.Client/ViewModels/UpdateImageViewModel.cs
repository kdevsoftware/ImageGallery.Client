using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ImageGallery.Client.ViewModels
{
    /// <summary>
    /// Update Image ViewModel.
    /// </summary>
    public class UpdateImageViewModel
    {
        /// <summary>
        /// Image Id (Guid).
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// File.
        /// </summary>
        public IFormFile File { get; set; }
    }
}
