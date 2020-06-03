﻿/*
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
    /// An implementation of this interface provides IIonHasher instances
    /// to an Ion hash implementation as needed.
    /// <p/>
    /// Implementations must be thread-safe.
    /// </summary>
    public interface IIonHasherProvider
    {
        /// <summary>
        /// Produces a new IIonHasher instance.
        /// </summary>
        /// <returns>The new IIonHasher instance.</returns>
        IIonHasher NewHasher();
    }
}
