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
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using IonDotnet;
    using IonDotnet.Builders;
    using IonDotnet.Tree;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class IonHashDataSource : Attribute, ITestDataSource
    {
        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            var dataList = new List<object[]>();

            var loader = IonLoader.Default;
            var file = DirStructure.IonHashDotnetTestFile("ion_hash_tests.ion");
            var ionHashTests = loader.Load(file);
            var testsEnumerator = ionHashTests.GetEnumerator();

            while (testsEnumerator.MoveNext())
            {
                IIonValue testCase = testsEnumerator.Current;

                string testName = "unknown";
                if (testCase.ContainsField("ion"))
                {
                    testName = testCase.GetField("ion").ToPrettyString();
                }

                IReadOnlyCollection<SymbolToken> annotations = testCase.GetTypeAnnotations();
                if (annotations.Count > 0)
                {
                    testName = annotations.ElementAt(0).Text;
                }

                IIonValue expect = testCase.GetField("expect");
                var expectEnumerator = expect.GetEnumerator();
                while (expectEnumerator.MoveNext())
                {
                    IIonValue expectedHashLog = expectEnumerator.Current;
                    String hasherName = expectedHashLog.FieldNameSymbol.Text;

                    object[] data = new object[] {
                        hasherName.Equals("identity") ? testName : testName + "." + hasherName,
                        testCase,
                        expectedHashLog,
                        TestIonHasherProvider.GetInstance(hasherName)
                    };
                    dataList.Add(data);
                }
            }

            return dataList;
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            return (string)data[0];
        }
    }
}
