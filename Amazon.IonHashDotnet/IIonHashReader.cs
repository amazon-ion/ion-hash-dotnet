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
    using Amazon.IonDotnet;

    /// <summary>
    /// IonReader extension that provides the hash of the IonValue just nexted
    /// * past or stepped out of, as defined by the Amazon Ion Hash Specification.
    /// <p/>
    /// Implementations of this interface are not thread-safe.
    /// </summary>
    /// <see cref="IIonReader"/>
    public interface IIonHashReader : IIonReader
    {
        /// <summary>
        /// Provides the hash of the Ion value just nexted past or stepped out of;
        /// hashes of partial Ion values are not provided.  If there is no current
        /// hash value, returns an empty array.
        /// <p/>
        /// Implementations must calculate the hash independently of how the Ion
        /// is traversed (e.g., the hash of a container must be identical whether
        /// the caller skips over it, steps into it, or any combination thereof).
        /// </summary>
        /// <returns>
        /// Array of bytes representing the hash of the IonValue just
        /// nexted past;  if there is no hash, returns an empty array.
        /// </returns>
        byte[] Digest();
    }
}
