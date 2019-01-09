using System.IO;
using System.Reflection;
using Xunit.Sdk;

namespace ImageGallery.Client.Test.UI.Fixtures.TestData
{
    public abstract class CsvDataAttribute : DataAttribute
    {
        public string FileName { get; set; }

        public string FileFullPath { get; set; }

        protected string GetTestFilePath()
        {
            if (!string.IsNullOrEmpty(FileFullPath))
            {
                return FileFullPath;
            }

            var assemblyName = Assembly.GetExecutingAssembly().Location;
            var assemblyDirectory = Path.GetDirectoryName(assemblyName);
            return Path.Combine(assemblyDirectory, FileName);
        }
    }
}
