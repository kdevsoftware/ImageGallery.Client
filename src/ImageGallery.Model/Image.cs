using System;

namespace ImageGallery.Model
{
    public class Image
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
        public string FileName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string PhotoId { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int? Width => 320;

        /// <summary>
        ///
        /// </summary>
        public int? Height => 240;
    }
}
