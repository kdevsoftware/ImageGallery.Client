﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Bogus;
using ImageGallery.Client.ViewModels;
using ImageGallery.Client.ViewModels.Album;
using ImageGallery.Model.Models.Albums;
using ImageGallery.Model.Models.Images;
using Image = ImageGallery.Model.Models.Images.Image;

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
                Title = image.Title,
                Category = image.Category,
            };
        }

        public static AddImageViewModel GetValidAddImage()
        {
            var image = GetImageData();
            return new AddImageViewModel
            {
                Title = image.Title,
                Category = image.Category,
            };
        }

        public static UpdateImageViewModel GetValidUpateImage()
        {
            var image = GetImageData();
            return new UpdateImageViewModel
            {
                Id = image.Id,
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

        public static List<AlbumImageSortItem> GetAlbumImagesSortList(int count)
        {
            var list = new List<AlbumImageSortItem>();
            var images = GetAlbumImages(count);
            foreach (var albumImage in images)
            {
                var albumImageSortItem = new AlbumImageSortItem
                {
                    ImageId = albumImage.Id,
                    Sort = albumImage.Sort ?? 0,
                };
                list.Add(albumImageSortItem);
            }

            return list;
        }

        public static byte[] GetImageFile(int maxXCells, int maxYCells, int cellXPosition, int cellYPosition)
        {
            using (var bmp = new System.Drawing.Bitmap(maxXCells, maxYCells))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    // Do your drawing here
                }

                var memStream = new MemoryStream();
                bmp.Save(memStream, ImageFormat.Jpeg);
                return memStream.ToArray();
            }
        }
    }
}
