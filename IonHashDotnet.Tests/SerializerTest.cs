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
