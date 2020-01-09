namespace IonHashDotnet.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
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
            var loader = IonLoader.Default;
            var file = DirStructure.IonHashTestFile("ion_hash_tests.ion");
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
                Debug.Write(expect.ToPrettyString());
            }

            return null;
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            return methodInfo.Name;
        }
    }
}
