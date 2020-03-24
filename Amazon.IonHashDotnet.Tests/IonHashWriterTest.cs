/*
 * Copyright 2020 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */

namespace Amazon.IonHashDotnet.Tests
{
    using System;
    using System.IO;
    using Amazon.IonDotnet;
    using Amazon.IonDotnet.Builders;
    using Amazon.IonDotnet.Tree;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class IonHashWriterTest
    {
        private static readonly IonLoader loader = IonLoader.Default;
        private static readonly FileInfo file = DirStructure.IonHashDotnetTestFile("ion_hash_tests.ion");

        [TestMethod]
        public void TestMiscMethods()
        {
            // coverage for Digest(), IsInStruct, SetFieldName(), AddTypeAnnotation()
            StringWriter stringWriter = new StringWriter();
            IIonHashWriter ihw = IonHashWriterBuilder
                .Standard()
                .WithHasherProvider(new IdentityIonHasherProvider())
                .WithWriter(IonTextWriterBuilder.Build(stringWriter))
                .Build();

            TestUtil.AssertEquals(new byte[] { }, ihw.Digest(), "Digests don't match.");

            ihw.WriteNull();
            TestUtil.AssertEquals(new byte[] { 0x0b, 0x0f, 0x0e }, ihw.Digest(), "Digests don't match.");

            ihw.StepIn(IonType.List);
            Assert.ThrowsException<InvalidOperationException>(ihw.Digest);

            ihw.WriteInt(5);
            Assert.ThrowsException<InvalidOperationException>(ihw.Digest);

            ihw.StepOut();
            TestUtil.AssertEquals(new byte[] { 0x0b, 0xb0, 0x0b, 0x20, 0x05, 0x0e, 0x0e }, ihw.Digest(), "Digests don't match.");

            ihw.WriteNull();
            TestUtil.AssertEquals(new byte[] { 0x0b, 0x0f, 0x0e }, ihw.Digest(), "Digests don't match.");

            Assert.IsFalse(ihw.IsInStruct);

            ihw.StepIn(IonType.Struct);
            Assert.IsTrue(ihw.IsInStruct);

            ihw.SetFieldName("hello");
            ihw.AddTypeAnnotation("ion");
            ihw.AddTypeAnnotation("hash");
            ihw.WriteSymbol("world");

            ihw.StepOut();
            Assert.IsFalse(ihw.IsInStruct);
            TestUtil.AssertEquals(new byte[] { 0x0b, 0xd0,
            0x0c, 0x0b, 0x70, 0x68, 0x65, 0x6c, 0x6c, 0x6f, 0x0c, 0x0e,     // hello:
            0x0c, 0x0b, 0xe0,
            0x0c, 0x0b, 0x70, 0x69, 0x6f, 0x6e, 0x0c, 0x0e,                 // ion::
            0x0c, 0x0b, 0x70, 0x68, 0x61, 0x73, 0x68, 0x0c, 0x0e,           // hash::
            0x0c, 0x0b, 0x70, 0x77, 0x6f, 0x72, 0x6c, 0x64, 0x0c, 0x0e,     // world
            0x0c, 0x0e,
            0x0e },
                ihw.Digest(),
                "Digests don't match.");

            ihw.Finish();

            Assert.AreEqual("null [5] null {hello:ion::hash::'world'}", stringWriter.ToString());
        }

        [TestMethod]
        public void TestExtraStepOut()
        {
            IIonHashWriter ihw = IonHashWriterBuilder
                .Standard()
                .WithHasherProvider(new IdentityIonHasherProvider())
                .WithWriter(IonTextWriterBuilder.Build(new StringWriter()))
                .Build();
            Assert.ThrowsException<InvalidOperationException>(ihw.StepOut);
        }

        [TestMethod]
        public void TestIonWriterContractWriteValue()
        {
            IIonValue ionHashTests = loader.Load(file);
            byte[] expected = ExerciseWriter(IonReaderBuilder.Build(ionHashTests), false, (r, w)=> { r.MoveNext(); w.WriteValue(r); });
            byte[] actual = ExerciseWriter(IonReaderBuilder.Build(ionHashTests), true, (r, w) => { r.MoveNext(); w.WriteValue(r); });
            Assert.IsTrue(expected.Length > 10);
            Assert.IsTrue(actual.Length > 10);
            TestUtil.AssertEquals(expected, actual, "Byte arrays mismatch.");
        }

        [TestMethod]
        public void TestIonWriterContractWriteValues()
        {
            IIonValue ionHashTests = loader.Load(file);
            byte[] expected = ExerciseWriter(IonReaderBuilder.Build(ionHashTests), false, (r, w) => { w.WriteValues(r); });
            byte[] actual = ExerciseWriter(IonReaderBuilder.Build(ionHashTests), true, (r, w) => { w.WriteValues(r); });
            Assert.IsTrue(expected.Length > 1000);
            Assert.IsTrue(actual.Length > 1000);
            TestUtil.AssertEquals(expected, actual, "Byte arrays mismatch.");
        }

        [TestMethod]
        public void TestUnresolvedSid()
        {
            // unresolved SIDs (such as SID 10 here) should result in an exception
            SymbolToken symbolUnresolvedSid = new SymbolToken(null, 10);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                IIonHashWriter writer = IonHashWriterBuilder
                    .Standard()
                    .WithHasherProvider(new IdentityIonHasherProvider())
                    .WithWriter(IonBinaryWriterBuilder.Build(memoryStream))
                    .Build();
                Assert.ThrowsException<UnknownSymbolException>(() => writer.WriteSymbolToken(symbolUnresolvedSid));
            }
        }

        private byte[] ExerciseWriter(IIonReader reader, bool useHashWriter, Action<IIonReader, IIonWriter> lambda)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                IIonWriter writer = IonBinaryWriterBuilder.Build(memoryStream);
                if (useHashWriter)
                {
                    writer = IonHashWriterBuilder
                        .Standard()
                        .WithHasherProvider(new IdentityIonHasherProvider())
                        .WithWriter(writer)
                        .Build();
                }
                lambda(reader, writer);
                writer.Finish();

                return memoryStream.ToArray();
            }
        }
    }
}
