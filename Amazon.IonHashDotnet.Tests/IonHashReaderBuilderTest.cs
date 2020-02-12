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
    using Amazon.IonDotnet;
    using Amazon.IonDotnet.Builders;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class IonHashReaderBuilderTest
    {
        private static readonly IIonHasherProvider hasherProvider = new CryptoIonHasherProvider("SHA-256");
        private static readonly IIonReader reader = IonReaderBuilder.Build("");

        [TestMethod]
        public void TestNullIonReader()
        {
            var ihrb = IonHashReaderBuilder.Standard().WithHasherProvider(hasherProvider);
            Assert.ThrowsException<ArgumentNullException>(ihrb.Build);
        }

        [TestMethod]
        public void TestNullHasherProvider()
        {
            var ihrb = IonHashReaderBuilder.Standard().WithReader(reader);
            Assert.ThrowsException<ArgumentNullException>(ihrb.Build);
        }

        [TestMethod]
        public void TestHappyCase()
        {
            var ihr = IonHashReaderBuilder.Standard().WithHasherProvider(hasherProvider).WithReader(reader).Build();
            Assert.IsNotNull(ihr);
        }
    }
}
