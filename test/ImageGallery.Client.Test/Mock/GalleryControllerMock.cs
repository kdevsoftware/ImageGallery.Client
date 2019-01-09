using ImageGallery.Client.Filters;
using ImageGallery.Client.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace ImageGallery.Client.Test.Mock
{
    public class GalleryControllerMock
    {
        private readonly ITestOutputHelper _output;

        public GalleryControllerMock(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void QueryStringBuilder_Test()
        {
            var galleryRequestModel1 = new GalleryRequestModel { Search = "Search_Test_1" };
            var x = galleryRequestModel1.ToQueryString();
            _output.WriteLine(x);

            var galleryRequestModel2 = new GalleryRequestModel { Category = "Category_Test_2", Search = "Search_Test_2" };
            var y = galleryRequestModel2.ToQueryString();
            _output.WriteLine(y);

            var galleryRequestModel3 = new GalleryRequestModel { };
            var z = galleryRequestModel3.ToQueryString();
            _output.WriteLine(z);

            Assert.Equal("?Search=Search_Test_1", x);
            Assert.Equal("?Category=Category_Test_2&Search=Search_Test_2", y);
            Assert.Equal(string.Empty, z);
        }
    }
}
