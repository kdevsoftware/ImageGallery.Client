using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Client.ViewModels.Album
{
    public class AlbumImageList
    {
        public AlbumImageList(Guid albumId)
        {
            this.AlbumId = albumId;
        }

        /// <summary>
        /// Album Id.
        /// </summary>
        public Guid AlbumId { get; set; }

        public List<AlbumImageSortItem> AlbumImageSortList { get; set; }
    }


    public class AlbumImageSortItem
    {
        public Guid ImageId { get; set; }

        public int Sort { get; set; }

    }
}
