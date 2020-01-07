namespace IonHashDotnet.Tests
{
    using System;
    using System.IO;
    using IonDotnet;
    using IonDotnet.Systems;
    using Xunit;

    public class IonHashWriterBuilderTest
    {
        private static readonly IIonHasherProvider HasherProvider = new CryptoIonHasherProvider("SHA-256");
        private static readonly StringWriter sw = new StringWriter();
        private static readonly IIonWriter Writer = IonTextWriterBuilder.Build(sw);

        [Fact]
        public void TestNullIonWriter()
        {
            var ihwb = IonHashWriterBuilder.Standard().WithHasherProvider(HasherProvider);
            Assert.Throws<ArgumentNullException>(ihwb.Build);
        }

        [Fact]
        public void TestNullHasherProvider()
        {
            var ihwb = IonHashWriterBuilder.Standard().WithWriter(Writer);
            Assert.Throws<ArgumentNullException>(ihwb.Build);
        }

        [Fact]
        public void TestHappyCase()
        {
            var ihr = IonHashWriterBuilder.Standard().WithHasherProvider(HasherProvider).WithWriter(Writer).Build();
            Assert.NotNull(ihr);
        }
    }
}
