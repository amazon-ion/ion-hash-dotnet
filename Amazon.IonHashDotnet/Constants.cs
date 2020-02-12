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
    internal static class Constants
    {
        internal static readonly byte BeginMarkerByte = 0x0B;
        internal static readonly byte EndMarkerByte = 0x0E;
        internal static readonly byte EscapeByte = 0x0C;
        internal static readonly byte[] BeginMarker = { BeginMarkerByte };
        internal static readonly byte[] EndMarker = { EndMarkerByte };

        internal static readonly byte[] TqAnnotatedValue = { 0xE0 };
    }
}
