using System;

namespace ImageGallery.Model.Models.Images
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
        ///  Flickr PhotoId
        /// </summary>
        public string PhotoId { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// Photo Widht.
        /// </summary>
        private int? _width;
        public int? Width
        {
            get
            {
                if (_width > _height || _width == null)
                {
                    return 320;
                }

                return 240;
            }
            set => _width = value;
        }

        /// <summary>
        ///  Photo Height.
        /// </summary>
        private int? _height;
        public int? Height
        {
            get => _height > _width ? 320 : 240;
            set => _height = value;
        }
    }
}
