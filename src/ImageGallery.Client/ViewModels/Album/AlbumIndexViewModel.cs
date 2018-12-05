using System.Collections.Generic;

namespace ImageGallery.Client.ViewModels.Album
{
    /// <summary>
    ///
    /// </summary>
    public class AlbumIndexViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumIndexViewModel"/> class.
        /// </summary>
        /// <param name="albums"></param>
        /// <param name="imagesUri"></param>
        public AlbumIndexViewModel(List<Model.Models.Albums.Album> albums, string imagesUri)
        {
            Albums = albums;
            ImagesUri = imagesUri;
        }

        /// <summary>
        ///
        /// </summary>
        public IEnumerable<Model.Models.Albums.Album> Albums { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public string ImagesUri { get; private set; }
    }
}
