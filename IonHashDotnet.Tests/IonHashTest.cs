namespace IonHashDotnet.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class IonHashTest
    {
        [DataTestMethod]
        [IonHashDataSource]
        public void TestIonHash(int expected, int actual)
        {
            Assert.AreEqual(expected, actual);
        }
    }
}
