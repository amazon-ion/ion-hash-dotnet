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
    /// User-provided hash function that is required by the Amazon Ion Hashing
    /// Specification.
    /// </summary>
    public interface IIonHasher
    {
        /// <summary>
        /// Updates the hash with the specified array of bytes.
        /// </summary>
        /// <param name="bytes">The bytes to hash.</param>
        void Update(byte[] bytes);

        /// <summary>
        /// Returns the computed hash bytes and resets any internal state
        /// so the hasher may be reused.
        /// </summary>
        /// <returns>The computed hash bytes.</returns>
        byte[] Digest();
    }
}
