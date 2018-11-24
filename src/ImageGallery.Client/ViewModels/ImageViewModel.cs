using System;
using Newtonsoft.Json;

namespace ImageGallery.Client.ViewModels
{
    /// <summary>
    /// Read Only Properties for Image.
    /// </summary>
    public class ImageViewModel
    {
        private readonly string _imageUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageViewModel"/> class.
        /// </summary>
        /// <param name="imagesUri"></param>
        public ImageViewModel(string imagesUri)
        {
            this._imageUri = imagesUri;
        }

        /// <summary>
        /// Image Id (Guid).
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Image Title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Image Category.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        ///  Image Filename.
        /// </summary>
        [JsonIgnore]
        public string FileName { get; set; }

        /// <summary>
        ///  Image Width.
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// Image Height.
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        ///  Image Url.
        /// </summary>
        public string ImageUrl => $"{_imageUri}{FileName}";
    }
}
