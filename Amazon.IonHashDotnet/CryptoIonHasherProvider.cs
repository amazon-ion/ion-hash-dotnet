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

namespace Amazon.IonHashDotnet
{
    /// <summary>
    /// IIonHasherProvider implementation based on System.Security.Cryptography.
    /// </summary>
    public class CryptoIonHasherProvider : IIonHasherProvider
    {
        private readonly string algorithm;

        public CryptoIonHasherProvider(string algorithm)
        {
            this.algorithm = algorithm;
        }

        public IIonHasher NewHasher()
        {
            return new CryptoIonHasher(this.algorithm);
        }
    }
}
