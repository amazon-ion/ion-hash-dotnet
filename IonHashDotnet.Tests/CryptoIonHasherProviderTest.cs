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
                0xd8, 0x38, 0x69, 0x1e, 0x5d, 0x4a, 0xd0, 0x68, 0x79,
                0xca, 0x72, 0x14, 0x42, 0xe8, 0x83, 0xd4
            };
            TestUtil.AssertEquals(expected, digest, "Digest don't match.");

            // Verify that the hasher resets after digest.
            TestUtil.AssertEquals(emptyHasherDigest, hasher.Digest(), "Digest don't match.");
        }
    }
}
