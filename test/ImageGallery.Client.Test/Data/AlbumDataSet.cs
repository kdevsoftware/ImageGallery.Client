using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using ImageGallery.Model.Models.Albums;

namespace ImageGallery.Client.Test.Data
{
    public static class AlbumDataSet
    {
        static AlbumDataSet()
        {
            Faker.GlobalUniqueIndex = 1;
        }

        public static Album GetAlbum()
        {
            var album = GetAlbumTableData(1).First();
            return album;
        }

        public static List<Album> GetAlbumTableData(int count)
        {
            //var imageFaker = new Faker<Image>()
            //    .RuleFor(c => c.Id, f => Guid.NewGuid())
            //    .RuleFor(c => c.Title, f => f.Name.Random.AlphaNumeric(10))
            //    .RuleFor(c => c.OwnerId, f => ownerId.ToString())
            //    .RuleFor(c => c.DataSource, f => "Mock");

            //var albumImagesFaker = new Faker<AlbumImage>()
            //    .RuleFor(t => t.AlbumId, f => Guid.NewGuid())
            //    .RuleFor(t => t.ImageId, f => Guid.NewGuid())
            //    .RuleFor(t => t.Sort, f => f.IndexGlobal).RuleFor(t => t.Image, f => imageFaker.Generate());

            var albumFaker = new Faker<Album>()
                .RuleFor(c => c.Id, f => Guid.NewGuid())
                .RuleFor(c => c.Title, f => f.Name.Random.AlphaNumeric(10))
                .RuleFor(c => c.DateCreated, f => f.Date.Past(2))
                .RuleFor(c => c.Description, f => f.Name.Random.AlphaNumeric(20));

            var albums = albumFaker.Generate(count);
            return albums;
        }

        public static List<AlbumTag> GetAlbumTagsTableData(int count)
        {
            var tagFaker = new Faker<AlbumTag>()
                .RuleFor(c => c.Id, f => Guid.NewGuid())
                .RuleFor(c => c.Tag, f => f.Name.Random.AlphaNumeric(10));

            var albums = tagFaker.Generate(count);
            return albums;
        }

        public static AlbumProperitesUpdate GetAlbumProperitesUpdateModel()
        {
            var album = GetAlbumTableData(1).First();
            var result = new AlbumProperitesUpdate
            {
                Title = album.Title,
                Description = album.Description,
            };

            return result;
        }

        public static AlbumMetaData GetAlbumMetaData(int tagCount)
        {
            var album = GetAlbumTableData(1).First();
            var result = new AlbumMetaData
            {
                Id = album.Id,
                Title = album.Title,
                Description = album.Description,
                DateCreated = album.DateCreated,
                AlbumTags = GetAlbumTagsTableData(tagCount),
            };

            return result;
        }

        public static List<AlbumImage> GetAlbumImageTableData(int count, string ownerId)
        {
            var fakeImages = new Faker<AlbumImage>()
                //.RuleFor(t => t.AlbumId, f => Guid.NewGuid())
                // .RuleFor(t => t.ImageId, f => Guid.NewGuid())
                .RuleFor(t => t.Sort, f => f.IndexGlobal);

            var albums = fakeImages.Generate(count);
            return albums;
        }

        //public static AlbumImage GetAlbumImageData()
        //{
        //    var result = GetAlbumImageTableData(1).First();
        //    return result;
        //}




        //public static AlbumImageModel GetAlbumImageModelData(Guid ownerId)
        //{
        //    var result = GetAlbumImageTableData(1, ownerId.ToString()).First();
        //    return new AlbumImageModel
        //    {
        //        AlbumId = result.AlbumId,
        //        ImageId = result.ImageId,
        //        Sort = result.Sort,
        //    };
        //}
    }
}
