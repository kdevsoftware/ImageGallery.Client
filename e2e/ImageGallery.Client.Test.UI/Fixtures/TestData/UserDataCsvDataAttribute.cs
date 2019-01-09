using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ImageGallery.Client.Test.UI.Fixtures.TestData
{
    public class UserDataCsvDataAttribute : CsvDataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            var filePath = GetTestFilePath();
            if (!File.Exists(filePath))
            {
                throw new ArgumentException($"File {filePath} cannot be found.");
            }

            return File.ReadAllLines(filePath).
                Select(u =>
                {
                    var userData = u.Split(new char[] { '|' });
                    return new object[] { userData[0], userData[1], userData[2] };
                });
        }
    }
}
