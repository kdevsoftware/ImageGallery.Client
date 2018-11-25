using ImageGallery.Client.Filters.Base;

namespace ImageGallery.Client.Filters
{
    /// <summary>
    /// Gallery Paging and Filtering Model.
    /// </summary>
    public class GalleryRequestModel : RequestModel
    {
        /// <summary>
        /// Photo Category Type.
        /// </summary>
        public string Category { get; set; }
    }
}
