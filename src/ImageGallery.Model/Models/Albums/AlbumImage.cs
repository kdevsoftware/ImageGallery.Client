using ImageGallery.Model.Models.Images;

namespace ImageGallery.Model.Models.Albums
{
    public class AlbumImage : Image
    {
        public bool IsPrimaryImage { get; set; }

        public int? Sort { get; set; }
    }
}
