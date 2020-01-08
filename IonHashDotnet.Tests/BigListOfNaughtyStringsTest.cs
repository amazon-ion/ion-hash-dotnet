namespace IonHashDotnet.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using IonDotnet;
    using IonDotnet.Systems;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BigListOfNaughtyStringsTest
    {
        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method)]
        public void Test(int a, int b)
        {
            Assert.AreEqual(a, b);
        }




        public static IEnumerable<object[]> GetData()
        {
            try
            {
                var file = IonTestFile("big_list_of_naughty_strings.txt");
                var fileStream = file.OpenRead();
                // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(fileStream))
                // Read the stream to a string, and write the string to the console.
                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();
                    Console.WriteLine(line);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }


            yield return new object[] { 1, 1 };
            yield return new object[] { 12, 30 };
            yield return new object[] { 14, 1 };
        }

        private static DirectoryInfo GetRootDir()
        {
            var dirInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (!string.Equals(dirInfo.Name, "ion-hash-dotnet", StringComparison.OrdinalIgnoreCase))
            {
                dirInfo = Directory.GetParent(dirInfo.FullName);
            }

            return dirInfo;
        }

        // ion-tests/iontestdata/
        public static DirectoryInfo IonTestDir()
        {
            var root = GetRootDir();
            return new DirectoryInfo(Path.Combine(
                root.FullName, "ion-hash-test"));
        }

        public static FileInfo IonTestFile(string relativePath)
        {
            var testDatDir = IonTestDir();
            return new FileInfo(Path.Combine(testDatDir.FullName, relativePath));
        }

    }
}
