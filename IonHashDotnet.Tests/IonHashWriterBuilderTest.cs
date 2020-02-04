namespace IonHashDotnet.Tests
{
    using System;
    using System.IO;
    using IonDotnet;
    using IonDotnet.Builders;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class IonHashWriterBuilderTest
    {
        private static readonly IIonHasherProvider hasherProvider = new CryptoIonHasherProvider("SHA-256");
        private static readonly IIonWriter writer = IonTextWriterBuilder.Build(new StringWriter());

        [TestMethod]
        public void TestNullIonWriter()
        {
            var ihwb = IonHashWriterBuilder.Standard().WithHasherProvider(hasherProvider);
            Assert.ThrowsException<ArgumentNullException>(ihwb.Build);
        }

        [TestMethod]
        public void TestNullHasherProvider()
        {
            var ihwb = IonHashWriterBuilder.Standard().WithWriter(writer);
            Assert.ThrowsException<ArgumentNullException>(ihwb.Build);
        }

        [TestMethod]
        public void TestHappyCase()
        {
            var ihr = IonHashWriterBuilder.Standard().WithHasherProvider(hasherProvider).WithWriter(writer).Build();
            Assert.IsNotNull(ihr);
        }
    }
}
