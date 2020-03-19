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
    using System.Collections.Generic;

    internal class StructSerializer : Serializer
    {
        private static readonly DigestComparer Comparer = new DigestComparer();
        private readonly Serializer scalarSerializer;
        private readonly List<byte[]> fieldHashes;

        internal StructSerializer(IIonHasher hashFunction, int depth, IIonHasherProvider hashFunctionProvider)
            : base(hashFunction, depth)
        {
            this.scalarSerializer = new Serializer(hashFunctionProvider.NewHasher(), depth + 1);
            this.fieldHashes = new List<byte[]>();
        }

        internal override void Scalar(IIonHashValue value, bool isInStruct)
        {
            this.scalarSerializer.HandleFieldName(value, isInStruct);
            this.scalarSerializer.Scalar(value, isInStruct);
            byte[] digest = this.scalarSerializer.Digest();
            this.AppendFieldHash(digest);
        }

        internal override void StepOut()
        {
            this.fieldHashes.Sort(Comparer);
            foreach (byte[] digest in this.fieldHashes)
            {
                this.Update(Escape(digest));
            }

            base.StepOut();
        }

        internal void AppendFieldHash(byte[] digest)
        {
            this.fieldHashes.Add(digest);
        }

        internal class DigestComparer : IComparer<byte[]>
        {
            public int Compare(byte[] a, byte[] b)
            {
                var i = 0;
                while (i < a.Length && i < b.Length)
                {
                    byte aByte = a[i];
                    byte bByte = b[i];
                    if (!aByte.Equals(bByte))
                    {
                        return aByte - bByte;
                    }

                    i++;
                }

                return a.Length - b.Length;
            }
        }
    }
}
