using ImageGallery.Client.Filters.Base;
using ImageGallery.Service.Helpers;

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
        [QueryString]
        public string Category { get; set; }

        /// <summary>
        /// Title Search Filter.
        /// </summary>
        [QueryString]
        public string Search { get; set; }
    }
}
