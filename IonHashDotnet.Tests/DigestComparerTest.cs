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

namespace IonHashDotnet.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DigestComparerTest
    {
        private readonly StructSerializer.DigestComparer comparer = new StructSerializer.DigestComparer();

        [TestMethod]
        public void TestNPE1()
        {
            Assert.ThrowsException<NullReferenceException>(() => comparer.Compare(null, null));
        }

        [TestMethod]
        public void TestNPE2()
        {
            Assert.ThrowsException<NullReferenceException>(() => comparer.Compare(null, new byte[] { }));
        }

        [TestMethod]
        public void TestNPE3()
        {
            Assert.ThrowsException<NullReferenceException>(() => comparer.Compare(new byte[] { }, null));
        }

        [TestMethod]
        public void TestIdentity()
        {
            byte[] emptyByteArray = { };
            Assert.AreEqual(0, comparer.Compare(emptyByteArray, emptyByteArray));

            byte[] bytes = { 0x01, 0x02, 0x03 };
            Assert.AreEqual(0, comparer.Compare(bytes, bytes));
        }

        [TestMethod]
        public void TestEquals()
        {
            Assert.AreEqual(0, comparer.Compare(new byte[] { 0x01, 0x02, 0x03 }, new byte[] { 0x01, 0x02, 0x03 }));
        }

        [TestMethod]
        public void LessThan()
        {
            Assert.AreEqual(-1, comparer.Compare(new byte[] { 0x01, 0x02, 0x03 }, new byte[] { 0x01, 0x02, 0x04 }));
        }

        [TestMethod]
        public void LessThanDueToLength()
        {
            Assert.AreEqual(-1, comparer.Compare(new byte[] { 0x01, 0x02, 0x03 },
                new byte[] { 0x01, 0x02, 0x03, 0x04 }));
        }

        [TestMethod]
        public void GreaterThanDueToLength()
        {
            Assert.AreEqual(1, comparer.Compare(new byte[] { 0x01, 0x02, 0x03, 0x04 },
                new byte[] { 0x01, 0x02, 0x03 }));
        }
    }
}
