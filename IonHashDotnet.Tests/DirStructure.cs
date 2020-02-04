/*
 * Copyright 2020 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */

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

        public static DirectoryInfo IonHashDotnetTestDir()
        {
            var root = GetRootDir();
            return new DirectoryInfo(Path.Combine(
                root.FullName, "IonHashDotnet.Tests"));
        }

        public static FileInfo IonHashTestFile(string relativePath)
        {
            var testDatDir = IonHashTestDir();
            return new FileInfo(Path.Combine(testDatDir.FullName, relativePath));
        }

        public static FileInfo IonHashDotnetTestFile(string relativePath)
        {
            var testDatDir = IonHashDotnetTestDir();
            return new FileInfo(Path.Combine(testDatDir.FullName, relativePath));
        }
    }
}
