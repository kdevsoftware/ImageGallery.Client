using System;
using System.Collections.Generic;
using System.Text;

namespace ImageGallery.Model.Models.Albums
{
    public class AlbumMetaData
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DateCreated { get; set; }

        public List<AlbumTag> AlbumTags { get; set; }
    }

    public class AlbumTag
    {
        public Guid Id { get; set; }

        public string Tag { get; set; }
    }
}
