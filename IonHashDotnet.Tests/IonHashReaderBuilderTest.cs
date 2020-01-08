namespace IonHashDotnet.Tests
{
    using System;
    using IonDotnet;
    using IonDotnet.Systems;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class IonHashReaderBuilderTest
    {
        private static readonly IIonHasherProvider HasherProvider = new CryptoIonHasherProvider("SHA-256");
        private static readonly IIonReader Reader = IonReaderBuilder.Build("");

        [TestMethod]
        public void TestNullIonReader()
        {
            var ihrb = IonHashReaderBuilder.Standard().WithHasherProvider(HasherProvider);
            Assert.ThrowsException<ArgumentNullException>(ihrb.Build);
        }

        [TestMethod]
        public void TestNullHasherProvider()
        {
            var ihrb = IonHashReaderBuilder.Standard().WithReader(Reader);
            Assert.ThrowsException<ArgumentNullException>(ihrb.Build);
        }

        [TestMethod]
        public void TestHappyCase()
        {
            var ihr = IonHashReaderBuilder.Standard().WithHasherProvider(HasherProvider).WithReader(Reader).Build();
            Assert.IsNotNull(ihr);
        }
    }
}
