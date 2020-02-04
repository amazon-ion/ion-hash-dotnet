namespace IonHashDotnet.Tests
{
    using System;
    using IonDotnet;
    using IonDotnet.Builders;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class IonHashReaderBuilderTest
    {
        private static readonly IIonHasherProvider hasherProvider = new CryptoIonHasherProvider("SHA-256");
        private static readonly IIonReader reader = IonReaderBuilder.Build("");

        [TestMethod]
        public void TestNullIonReader()
        {
            var ihrb = IonHashReaderBuilder.Standard().WithHasherProvider(hasherProvider);
            Assert.ThrowsException<ArgumentNullException>(ihrb.Build);
        }

        [TestMethod]
        public void TestNullHasherProvider()
        {
            var ihrb = IonHashReaderBuilder.Standard().WithReader(reader);
            Assert.ThrowsException<ArgumentNullException>(ihrb.Build);
        }

        [TestMethod]
        public void TestHappyCase()
        {
            var ihr = IonHashReaderBuilder.Standard().WithHasherProvider(hasherProvider).WithReader(reader).Build();
            Assert.IsNotNull(ihr);
        }
    }
}
