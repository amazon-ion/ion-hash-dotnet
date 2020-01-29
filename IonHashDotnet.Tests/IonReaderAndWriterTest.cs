namespace IonHashDotnet.Tests
{
    using System.IO;
    using IonDotnet;
    using IonDotnet.Builders;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class IonReaderAndWriterTest
    {
        [TestMethod]
        public void TestNoFieldNameInCurrentHash()
        {
            AssertNoFieldnameInCurrentHash("null", new byte[] { 0x0b, 0x0f, 0x0e });
            AssertNoFieldnameInCurrentHash("false", new byte[] { 0x0b, 0x10, 0x0e });
            AssertNoFieldnameInCurrentHash("5", new byte[] { 0x0b, 0x20, 0x05, 0x0e });
            AssertNoFieldnameInCurrentHash("2e0", new byte[] { 0x0b, 0x40, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0e });
            AssertNoFieldnameInCurrentHash("1234.500", new byte[] { 0x0b, 0x50, 0xc3, 0x12, 0xd6, 0x44, 0x0e });
            AssertNoFieldnameInCurrentHash("hi", new byte[] { 0x0b, 0x70, 0x68, 0x69, 0x0e });
            AssertNoFieldnameInCurrentHash("\"hi\"", new byte[] { 0x0b, 0x80, 0x68, 0x69, 0x0e });
            AssertNoFieldnameInCurrentHash("{{\"hi\"}}", new byte[] { 0x0b, 0x90, 0x68, 0x69, 0x0e, });
            AssertNoFieldnameInCurrentHash("{{aGVsbG8=}}", new byte[] { 0x0b, 0xa0, 0x68, 0x65, 0x6c, 0x6c, 0x6f, 0x0e });
            AssertNoFieldnameInCurrentHash("[1,2,3]", new byte[] { 0x0b, 0xb0, 0x0b, 0x20, 0x01, 0x0e, 0x0b, 0x20, 0x02, 0x0e, 0x0b, 0x20, 0x03, 0x0e, 0x0e });
            AssertNoFieldnameInCurrentHash("(1 2 3)", new byte[] { 0x0b, 0xc0, 0x0b, 0x20, 0x01, 0x0e, 0x0b, 0x20, 0x02, 0x0e, 0x0b, 0x20, 0x03, 0x0e, 0x0e });
            AssertNoFieldnameInCurrentHash("{a:1,b:2,c:3}", new byte[] { 0x0b, 0xd0, 0x0c, 0x0b, 0x70, 0x61, 0x0c, 0x0e, 0x0c, 0x0b, 0x20, 0x01, 0x0c, 0x0e, 0x0c,
                   0x0b, 0x70, 0x62, 0x0c, 0x0e, 0x0c, 0x0b, 0x20, 0x02, 0x0c, 0x0e, 0x0c, 0x0b, 0x70, 0x63, 0x0c, 0x0e, 0x0c, 0x0b, 0x20, 0x03, 0x0c, 0x0e, 0x0e });
            AssertNoFieldnameInCurrentHash("hi::7", new byte[] { 0x0b, 0xe0, 0x0b, 0x70, 0x68, 0x69, 0x0e, 0x0b, 0x20, 0x07, 0x0e, 0x0e });
        }

        private void AssertNoFieldnameInCurrentHash(string value, byte[] expectedBytes)
        {
            var reader = IonReaderBuilder.Build(value);
            reader.MoveNext();

            MemoryStream memoryStream = new MemoryStream();
            var writer = IonBinaryWriterBuilder.Build(memoryStream);
            writer.StepIn(IonType.Struct);

            IIonHashWriter ihw = IonHashWriterBuilder
                .Standard()
                .WithHasherProvider(new IdentityIonHasherProvider())
                .WithWriter(writer)
                .Build();
            ihw.SetFieldName("field_name");
            ihw.WriteValue(reader);
            byte[] actual = ihw.Digest();
            TestUtil.AssertEquals(expectedBytes, actual, "Digest arrays mismatch");
            writer.StepOut();

            ihw.Finish();
            writer.Finish();
            memoryStream.Flush();
            byte[] bytes = memoryStream.ToArray();
            memoryStream.Close();

            reader = IonReaderBuilder.Build(bytes);
            reader.MoveNext();
            reader.StepIn();
            IIonHashReader ihr = IonHashReaderBuilder
                .Standard()
                .WithHasherProvider(new IdentityIonHasherProvider())
                .WithReader(reader)
                .Build();
            ihr.MoveNext();   //List 
            ihr.MoveNext();    //none
            actual = ihr.Digest();
            TestUtil.AssertEquals(expectedBytes, actual, "Digest arrays mismatch");
        }

        [TestMethod]
        public void FieldNameAsymmetry()
        {
            var memoryStream = new MemoryStream();
            var writer = IonBinaryWriterBuilder.Build(memoryStream);

            IIonHashWriter ihw = IonHashWriterBuilder
                .Standard()
                .WithHasherProvider(new IdentityIonHasherProvider())
                .WithWriter(writer)
                .Build();

            // A nested struct: {a:{b:1}}
            writer.StepIn(IonType.Struct);
            writer.SetFieldName("a");
            ihw.StepIn(IonType.Struct);
            ihw.SetFieldName("b");
            ihw.WriteInt(1);
            ihw.StepOut();
            byte[] writeHash = ihw.Digest();
            ihw.Flush();
            writer.StepOut();
            writer.Flush();

            var loader = IonLoader.Default;
            var ionValue = loader.Load(memoryStream.ToArray())  //Datagram
                .GetElementAt(0)                                //first struct
                .GetElementAt(0);                               //inner struct

            IIonReader reader = IonReaderBuilder.Build(ionValue);
            IIonHashReader ihr = IonHashReaderBuilder
                .Standard()
                .WithReader(reader)
                .WithHasherProvider(new IdentityIonHasherProvider())
                .Build();
            ihr.MoveNext();  // struct
            ihr.MoveNext();  // none

            TestUtil.AssertEquals(writeHash, ihr.Digest(), "Digest arrays mismatch");
        }
    }
}
