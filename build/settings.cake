public static class Settings
{
    public static string ProjectName => "ImageGalleryClient";

    public static string SonarUrl => "http://sonar.navigatorglass.com:9000";

    public static string SonarKey => "ImageGalleryClient";

    public static string SonarName => "ImageGalleryClient";

    public static string SonarExclude => "/d:sonar.exclusions=**/wwwroot/**,*.js";

    public static string SonarExcludeDuplications => "/d:sonar.cpd.exclusions=**/GalleryContextExtensions.cs";

    /// ./test/**/*.csproj (All)
    public static string UnitTestingProjects  => "./test/ImageGallery.Client.Test/*.csproj";

    public static string E2ETestingProjects  => "./test/ImageGallery.Client.Test.UI/*.csproj";

}
