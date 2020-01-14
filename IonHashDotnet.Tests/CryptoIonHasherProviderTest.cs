namespace IonHashDotnet.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CryptoIonHasherProviderTest
    {
        [TestMethod]
        public void TestInvalidAlgorithm()
        {
            Assert.ThrowsException<ArgumentException>(new CryptoIonHasherProvider("invalid algorithm").NewHasher);
        }

        [TestMethod]
        public void TestHasher()
        {
            // Using flawed MD5 algorithm FOR TEST PURPOSES ONLY
            IIonHasherProvider hasherProvider = new CryptoIonHasherProvider("MD5");
            IIonHasher hasher = hasherProvider.NewHasher();
            byte[] emptyHasherDigest = hasher.Digest();

            hasher.Update(new byte[] { 0x0f });
            byte[] digest = hasher.Digest();
            byte[] expected = {
                (byte)0xd8, 0x38, 0x69, 0x1e, 0x5d, 0x4a, (byte)0xd0, 0x68, 0x79,
                (byte)0xca, 0x72, 0x14, 0x42, (byte)0xe8, (byte)0x83, (byte)0xd4
            };
            TestUtil.AssertEquals(expected, digest, "Digest don't match.");

            // Verify that the hasher resets after digest.
            TestUtil.AssertEquals(emptyHasherDigest, hasher.Digest(), "Digest don't match.");
        }
    }
}
