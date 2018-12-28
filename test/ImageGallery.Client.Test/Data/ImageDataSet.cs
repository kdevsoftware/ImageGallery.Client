using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using ImageGallery.Client.ViewModels;
using ImageGallery.Model.Models.Images;

namespace ImageGallery.Client.Test.Data
{
    public static class ImageDataSet
    {
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
                .RuleFor(c => c.Title, f => f.Name.Random.AlphaNumeric(10))
                .RuleFor(c => c.Category, f => f.Name.Random.AlphaNumeric(5))
                .RuleFor(c => c.DataSource, f => "Mock");

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

        public static ImageProperitesUpdate GetValidImageProperitesUpdate()
        {
            var image = GetImageData();
            return new ImageProperitesUpdate
            {
                Category = image.Category,
                Title = image.Title,
            };
        }
    }
}
