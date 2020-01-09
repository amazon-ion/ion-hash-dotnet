namespace IonHashDotnet.Tests
{
    using System;
    using System.IO;

    internal static class DirStructure
    {
        private static DirectoryInfo GetRootDir()
        {
            var dirInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (!string.Equals(dirInfo.Name, "ion-hash-dotnet", StringComparison.OrdinalIgnoreCase))
            {
                dirInfo = Directory.GetParent(dirInfo.FullName);
            }

            return dirInfo;
        }

        public static DirectoryInfo IonHashTestDir()
        {
            var root = GetRootDir();
            return new DirectoryInfo(Path.Combine(
                root.FullName, "ion-hash-test"));
        }

        public static FileInfo IonHashTestFile(string relativePath)
        {
            var testDatDir = IonHashTestDir();
            return new FileInfo(Path.Combine(testDatDir.FullName, relativePath));
        }

    }
}
