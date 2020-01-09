namespace IonHashDotnet.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class TestUtil
    {
        public static void AssertEquals(byte[] expected, byte[] actual, string message)
        {
            Assert.AreEqual(expected, actual, message);
        }
    }
}
