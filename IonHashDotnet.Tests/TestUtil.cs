namespace IonHashDotnet.Tests
{
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public static class TestUtil
    {
        public static void AssertEquals(byte[] expected, byte[] actual, string message)
        {
            Assert.AreEqual(expected.Length, actual.Length, message);
            Assert.AreEqual(BytesToHex(expected), BytesToHex(actual), message);
        }

        private static string BytesToHex(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.Append(string.Format("{0}2x ", b));
            }
            return sb.ToString();
        }
    }
}
