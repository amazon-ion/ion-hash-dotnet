namespace IonHashDotnet.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public static class TestUtil
    {
        public static void AssertEquals(byte[] expected, byte[] actual, string message)
        {
            Assert.AreEqual(expected.Length, actual.Length, message);

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], message);
            }
        }
    }
}

