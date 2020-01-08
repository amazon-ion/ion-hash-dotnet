using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IonHashDotnet.Tests
{
    public class IonHashDataSource : Attribute, ITestDataSource
    {
        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            yield return new object[] { 1, 4 };
            yield return new object[] { 1, 1 };
            yield return new object[] { 4, 4 };
            Console.WriteLine("called this once");
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            return data[0].ToString();
        }
    }
}
