using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ImageGallery.Client.Test.UI.Fixtures.TestData
{
    public class ImageDataCsvDataAttribute : CsvDataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            var filePath = GetTestFilePath();
            if (!File.Exists(filePath))
            {
                throw new ArgumentException($"File {filePath} cannot be found.");
            }

            var results = File.ReadAllLines(filePath).
                Where(i => i.Split(new char[] { '|' }).Length == 5).
                Select(i =>
                {
                    var userData = i.Split(new char[] { '|' });
                    return new object[]
                    {
                        userData[0],
                        userData[1],
                        userData[2],
                        userData[3],
                        userData[4],
                    };
                });

            return results;
        }
    }
}
