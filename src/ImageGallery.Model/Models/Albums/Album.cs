using System;

namespace ImageGallery.Model
{
    public class Album
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DateCreated { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string FileName { get; set; }

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
