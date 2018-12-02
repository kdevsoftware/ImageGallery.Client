using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ImageGallery.Model.Models.Images
{
    /// <summary>
    /// 
    /// </summary>
    public class ImageUpdate
    {
        /// <summary>
        ///
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///
        /// </summary>
        [Required]
        public byte[] Bytes { get; set; }
    }
}
