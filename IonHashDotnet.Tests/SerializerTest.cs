namespace IonHashDotnet.Tests
{
    using System;
    using IonHashDotnet;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SerializerTest
    {
        [TestMethod]
        public void Escape()
        {
            // null case
            Assert.IsNull(Serializer.Escape(null));

            // happy cases
            byte[] empty = {};
            TestUtil.AssertEquals(empty, Serializer.Escape(empty), "Byte arrays mismatch.");

            byte[] bytes = { 0x10, 0x11, 0x12, 0x13 };
            TestUtil.AssertEquals(bytes, Serializer.Escape(bytes), "Byte arrays mismatch.");

            // escape cases
            TestUtil.AssertEquals(new byte[] { 0x0C, 0x0B }, Serializer.Escape(new byte[] { 0x0B }), "Byte arrays mismatch.");
            TestUtil.AssertEquals(new byte[] { 0x0C, 0x0E }, Serializer.Escape(new byte[] { 0x0E }), "Byte arrays mismatch.");
            TestUtil.AssertEquals(new byte[] { 0x0C, 0x0C }, Serializer.Escape(new byte[] { 0x0C }), "Byte arrays mismatch.");

            TestUtil.AssertEquals(new byte[] { 0x0C, 0x0B, 0x0C, 0x0E, 0x0C, 0x0C },
                Serializer.Escape(new byte[] {       0x0B,       0x0E,       0x0C }), "Byte arrays mismatch.");

            TestUtil.AssertEquals(new byte[] { 0x0C, 0x0C, 0x0C, 0x0C },
                Serializer.Escape(new byte[] {       0x0C,       0x0C }), "Byte arrays mismatch.");

            TestUtil.AssertEquals(new byte[] { 0x0C, 0x0C, 0x10, 0x0C, 0x0C, 0x11, 0x0C, 0x0C, 0x12, 0x0C, 0x0C },
                Serializer.Escape(new byte[] {       0x0C, 0x10,       0x0C, 0x11,       0x0C, 0x12,       0x0C }),
                "Byte arrays mismatch.");
        }
    }
}
