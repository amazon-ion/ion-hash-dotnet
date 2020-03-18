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
    using System;
    using System.Collections.Generic;
    using Amazon.IonDotnet;

    internal class Hasher
    {
        private readonly IIonHasherProvider hasherProvider;
        private readonly Stack<Serializer> hasherStack;
        private Serializer currentHasher;

        internal Hasher(IIonHasherProvider hasherProvider)
        {
            this.hasherProvider = hasherProvider;
            this.currentHasher = new Serializer(hasherProvider.NewHasher());
            this.hasherStack = new Stack<Serializer>();
            this.hasherStack.Push(this.currentHasher);
        }

        internal void Scalar(IIonHashValue ionValue, bool isInStruct)
        {
            this.currentHasher.Scalar(ionValue, isInStruct);
        }

        internal void StepIn(IIonHashValue ionValue, bool isInStruct)
        {
            IIonHasher hashFunction = this.currentHasher.HashFunction;
            if (this.currentHasher is StructSerializer)
            {
                hashFunction = this.hasherProvider.NewHasher();
            }

            if (ionValue.CurrentType == IonType.Struct)
            {
                this.currentHasher = new StructSerializer(hashFunction, this.hasherProvider);
            }
            else
            {
                this.currentHasher = new Serializer(hashFunction);
            }

            this.hasherStack.Push(this.currentHasher);
            this.currentHasher.StepIn(ionValue, isInStruct);
        }

        internal void StepOut()
        {
            if (this.Depth() == 0)
            {
                throw new InvalidOperationException("Hasher cannot StepOut any further");
            }

            this.currentHasher.StepOut();
            Serializer poppedHasher = this.hasherStack.Pop();
            this.currentHasher = this.hasherStack.Peek();

            if (this.currentHasher is StructSerializer)
            {
                byte[] digest = poppedHasher.Digest();
                ((StructSerializer)this.currentHasher).AppendFieldHash(digest);
            }
        }

        internal byte[] Digest()
        {
            if (this.Depth() != 0)
            {
                throw new InvalidOperationException("A digest may only be provided at the same depth hashing started");
            }

            return this.currentHasher.Digest();
        }

        private int Depth()
        {
            return this.hasherStack.Count - 1;
        }
    }
}
