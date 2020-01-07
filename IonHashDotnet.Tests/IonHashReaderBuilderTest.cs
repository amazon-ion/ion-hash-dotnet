namespace IonHashDotnet.Tests
{
    using System;
    using IonDotnet;
    using IonDotnet.Systems;
    using Xunit;

    public class IonHashReaderBuilderTest
    {
        private static readonly IIonHasherProvider HasherProvider = new CryptoIonHasherProvider("SHA-256");
        private static readonly IIonReader Reader = IonReaderBuilder.Build("");

        [Fact]
        public void TestNullIonReader()
        {
            var ihrb = IonHashReaderBuilder.Standard().WithHasherProvider(HasherProvider);
            Assert.Throws<ArgumentNullException>(ihrb.Build);
        }

        [Fact]
        public void TestNullHasherProvider()
        {
            var ihrb = IonHashReaderBuilder.Standard().WithReader(Reader);
            Assert.Throws<ArgumentNullException>(ihrb.Build);
        }

        [Fact]
        public void TestHappyCase()
        {
            var ihr = IonHashReaderBuilder.Standard().WithHasherProvider(HasherProvider).WithReader(Reader).Build();
            Assert.NotNull(ihr);
        }
    }
}
