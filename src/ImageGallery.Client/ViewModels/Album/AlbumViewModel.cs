using System;

namespace ImageGallery.Client.ViewModels.Album
{
    /// <summary>
    ///
    /// </summary>
    public class AlbumViewModel
    {
        /// <summary>
        ///
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///
        /// </summary>
        public DateTime DateCreated { get; set; }
    }
}
