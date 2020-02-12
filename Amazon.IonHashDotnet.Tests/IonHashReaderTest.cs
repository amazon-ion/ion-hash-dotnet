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
    public class IonHashReaderTest
    {
        private static readonly IonLoader loader = IonLoader.Default;

        [TestMethod]
        public void TestEmptyString()
        {
            IIonHashReader ihr = IonHashReaderBuilder
                .Standard()
                .WithHasherProvider(new IdentityIonHasherProvider())
                .WithReader(IonReaderBuilder.Build(""))
                .Build();

            Assert.AreEqual(IonType.None, ihr.MoveNext());
            TestUtil.AssertEquals(new byte[] { }, ihr.Digest(), "Digests don't match.");
            Assert.AreEqual(IonType.None, ihr.MoveNext());
            TestUtil.AssertEquals(new byte[] { }, ihr.Digest(), "Digests don't match.");
        }

        [TestMethod]
        public void TestTopLevelValues()
        {
            IIonHashReader ihr = IonHashReaderBuilder
                .Standard()
                .WithHasherProvider(new IdentityIonHasherProvider())
                .WithReader(IonReaderBuilder.Build("1 2 3"))
                .Build();

            Assert.AreEqual(IonType.Int, ihr.MoveNext());
            TestUtil.AssertEquals(new byte[] { }, ihr.Digest(), "Digests don't match.");

            Assert.AreEqual(IonType.Int, ihr.MoveNext());
            TestUtil.AssertEquals(new byte[] { 0x0b, 0x20, 0x01, 0x0e }, ihr.Digest(), "Digests don't match.");

            Assert.AreEqual(IonType.Int, ihr.MoveNext());
            TestUtil.AssertEquals(new byte[] { 0x0b, 0x20, 0x02, 0x0e }, ihr.Digest(), "Digests don't match.");

            Assert.AreEqual(IonType.None, ihr.MoveNext());
            TestUtil.AssertEquals(new byte[] { 0x0b, 0x20, 0x03, 0x0e }, ihr.Digest(), "Digests don't match.");

            Assert.AreEqual(IonType.None, ihr.MoveNext());
            TestUtil.AssertEquals(new byte[] { }, ihr.Digest(), "Digests don't match.");
        }

        [TestMethod]
        public void TestConsumeRemainderPartialConsume()
        {
            Consume((ihr) =>
            {
                ihr.MoveNext();
                ihr.StepIn();
                ihr.MoveNext();
                ihr.MoveNext();
                ihr.MoveNext();
                ihr.StepIn();
                ihr.MoveNext();
                ihr.StepOut();  // we've only partially consumed the struct
                ihr.StepOut();  // we've only partially consumed the list
            });
        }

        [TestMethod]
        public void TestConsumeRemainderStepInStepOutNested()
        {
            Consume((ihr) =>
            {
                ihr.MoveNext();
                ihr.StepIn();
                ihr.MoveNext();
                ihr.MoveNext();
                ihr.MoveNext();
                ihr.StepIn();
                ihr.StepOut();  // we haven't consumed ANY of the struct
                ihr.StepOut();  // we've only partially consumed the list
            });
        }

        [TestMethod]
        public void TestConsumeRemainderStepInNextStepOut()
        {
            this.Consume((ihr) =>
            {
                ihr.MoveNext();
                ihr.StepIn();
                ihr.MoveNext();
                ihr.StepOut();  // we've partially consumed the list
            });
        }

        [TestMethod]
        public void TestConsumeRemainderStepInStepOutTopLevel()
        {
            this.Consume((ihr) =>
            {
                ihr.MoveNext();
                TestUtil.AssertEquals(new byte[] { }, ihr.Digest(), "Digests don't match.");

                ihr.StepIn();
                Assert.ThrowsException<InvalidOperationException>(ihr.Digest);
                ihr.StepOut();  // we haven't consumed ANY of the list
            });
        }

        [TestMethod]
        public void TestConsumeRemainderSingleNext()
        {
            Consume((ihr) =>
            {
                ihr.MoveNext();
                ihr.MoveNext();
            });
        }

        [TestMethod]
        public void TestUnresolvedSid()
        {
            // unresolved SIDs (such as SID 10 here) should result in an exception
            IIonValue ionContainer = loader.Load("(0xd3 0x8a 0x21 0x01)");

            byte[] ionBinary = IonHashTest.ContainerToBytes(ionContainer.GetElementAt(0));

            IIonHashReader ihr = IonHashReaderBuilder
                .Standard()
                .WithHasherProvider(new IdentityIonHasherProvider())
                .WithReader(IonReaderBuilder.Build(ionBinary))
                .Build();
            ihr.MoveNext();
            Assert.ThrowsException<UnknownSymbolException>(() => ihr.MoveNext());
        }

        [TestMethod]
        public void TestIonReaderContract()
        {
            FileInfo file = DirStructure.IonHashDotnetTestFile("ion_hash_tests.ion");
            IIonValue ionHashTests = loader.Load(file);

            IIonReader ir = IonReaderBuilder.Build(ionHashTests);

            IIonHashReader ihr = IonHashReaderBuilder
                .Standard()
                .WithHasherProvider(new IdentityIonHasherProvider())
                .WithReader(IonReaderBuilder.Build(ionHashTests))
                .Build();

            ReaderCompare.Compare(ir, ihr);
        }

        private void Consume(Action<IIonHashReader> lambda)
        {
            IIonHashReader ihr = IonHashReaderBuilder
                .Standard()
                .WithHasherProvider(new IdentityIonHasherProvider())
                .WithReader(IonReaderBuilder.Build("[1,2,{a:3,b:4},5]"))
                .Build();
            TestUtil.AssertEquals(new byte[] { }, ihr.Digest(), "Digests don't match.");
            lambda(ihr);
            TestUtil.AssertEquals(new byte[] { 0x0b, 0xb0,
                0x0b, 0x20, 0x01, 0x0e,
                0x0b, 0x20, 0x02, 0x0e,
                0x0b, 0xd0,
                0x0c, 0x0b, 0x70, 0x61, 0x0c, 0x0e, 0x0c, 0x0b, 0x20, 0x03, 0x0c, 0x0e,
                0x0c, 0x0b, 0x70, 0x62, 0x0c, 0x0e, 0x0c, 0x0b, 0x20, 0x04, 0x0c, 0x0e,
                0x0e,
                0x0b, 0x20, 0x05, 0x0e,
                0x0e}, ihr.Digest(), "Digests don't match.");

            Assert.AreEqual(IonType.None, ihr.MoveNext());
            TestUtil.AssertEquals(new byte[] { }, ihr.Digest(), "Digests don't match.");
        }
    }
}
