using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using ImageGallery.Client.ViewModels;
using ImageGallery.Model.Models.Albums;
using ImageGallery.Model.Models.Images;

namespace ImageGallery.Client.Test.Data
{
    public static class ImageDataSet
    {
        static ImageDataSet()
        {
            Faker.GlobalUniqueIndex = 0;
        }

        public static Image GetImageData()
        {
            var result = GetImageTableData(1).First();
            return result;
        }

        //public static ImageModel GetImageModelData(string ownerId)
        //{
        //    var result = GetImageTableData(1, ownerId).First();
        //    var imageModel = new ImageModel
        //    {
        //        Id = result.Id,
        //        Title = result.Title,
        //        FileName = result.FileName,
        //        Category = result.Category,
        //        Width = result.Width,
        //        Height = result.Height,
        //        PhotoId = result.PhotoId,
        //        DataSource = result.DataSource,
        //    };

        //    return imageModel;
        //}

        public static List<Image> GetImageTableData(int count)
        {
            var imageFaker = new Faker<Image>()
                .RuleFor(c => c.Id, f => Guid.NewGuid())
                .RuleFor(c => c.FileName, f => $"{Guid.NewGuid()}.jpg")
                .RuleFor(c => c.Title, f => f.Name.Random.AlphaNumeric(10))
                .RuleFor(c => c.Category, f => f.Name.Random.AlphaNumeric(5))
                .RuleFor(c => c.DataSource, f => "Mock")
                .RuleFor(c => c.PhotoId, f => f.Name.Random.AlphaNumeric(10));

            var images = imageFaker.Generate(count);
            return images;
        }

        public static EditImageViewModel GetValidEditImage()
        {
            var image = GetImageData();
            return new EditImageViewModel
            {
                Id = image.Id,
                Category = image.Category,
                Title = image.Title,
            };
        }

        public static ImageViewModel GetImageViewModel()
        {
            var image = GetImageData();
            return new ImageViewModel(CommonConstants.ImagesUri)
            {
                Id = image.Id,
                Title = image.Title,
                Category = image.Category,
                FileName = image.FileName,
                Width = image.Width,
                Height = image.Height,
            };
        }

        public static ImageProperitesUpdate GetValidImageProperitesUpdate()
        {
            var image = GetImageData();
            return new ImageProperitesUpdate
            {
                Category = image.Category,
                Title = image.Title,
            };
        }

        public static List<AlbumImage> GetAlbumImages(int count)
        {
            List<Image> images = GetImageTableData(count);

            var albumImages = new List<AlbumImage>();
            int index = 0;

            foreach (var image in images)
            {
                var albumImage = new AlbumImage
                {
                    Id = image.Id,
                    Title = image.Title,
                    Category = image.Category,
                    DataSource = image.DataSource,
                    FileName = image.FileName,
                    IsPrimaryImage = index == 0,
                    PhotoId = image.PhotoId,
                    Sort = ++index,
                };
                albumImages.Add(albumImage);
            }

            return albumImages;
        }
    }
}
