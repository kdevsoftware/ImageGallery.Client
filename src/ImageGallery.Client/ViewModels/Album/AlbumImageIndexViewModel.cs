using System.Collections.Generic;
using ImageGallery.Model.Models.Albums;

namespace ImageGallery.Client.ViewModels.Album
{
    /// <summary>
    ///
    /// </summary>
    public class AlbumImageIndexViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumImageIndexViewModel"/> class.
        /// </summary>
        /// <param name="images"></param>
        /// <param name="imagesUri"></param>
        public AlbumImageIndexViewModel(List<AlbumImage> images, string imagesUri)
        {
            Images = images;
            ImagesUri = imagesUri;
        }

        /// <summary>
        ///
        /// </summary>
        public IEnumerable<AlbumImage> Images { get; private set; }

        /// <summary>
        /// Image Root Folder.
        /// </summary>
        public string ImagesUri { get; private set; }
    }
}
