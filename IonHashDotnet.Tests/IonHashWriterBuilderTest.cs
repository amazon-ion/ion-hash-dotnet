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
        private static readonly IIonHasherProvider HasherProvider = new CryptoIonHasherProvider("SHA-256");
        private static readonly IIonWriter Writer = IonTextWriterBuilder.Build(new StringWriter());

        [TestMethod]
        public void TestNullIonWriter()
        {
            var ihwb = IonHashWriterBuilder.Standard().WithHasherProvider(HasherProvider);
            Assert.ThrowsException<ArgumentNullException>(ihwb.Build);
        }

        [TestMethod]
        public void TestNullHasherProvider()
        {
            var ihwb = IonHashWriterBuilder.Standard().WithWriter(Writer);
            Assert.ThrowsException<ArgumentNullException>(ihwb.Build);
        }

        [TestMethod]
        public void TestHappyCase()
        {
            var ihr = IonHashWriterBuilder.Standard().WithHasherProvider(HasherProvider).WithWriter(Writer).Build();
            Assert.IsNotNull(ihr);
        }
    }
}
