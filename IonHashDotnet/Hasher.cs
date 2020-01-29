namespace IonHashDotnet
{
    using System;
    using System.Collections.Generic;
    using IonDotnet;

    internal class Hasher
    {
        private readonly IIonHasherProvider hasherProvider;
        private readonly Stack<Serializer> hasherStack;
        private Serializer currentHasher;

        internal Hasher(IIonHasherProvider hasherProvider)
        {
            this.hasherProvider = hasherProvider;
            this.currentHasher = new Serializer(hasherProvider.NewHasher(), 0);
            this.hasherStack = new Stack<Serializer>();
            this.hasherStack.Push(this.currentHasher);
        }

        internal void Scalar(IIonHashValue ionValue)
        {
            this.currentHasher.Scalar(ionValue);
        }

        internal void StepIn(IIonHashValue ionValue)
        {
            IIonHasher hashFunction = this.currentHasher.HashFunction;
            if (this.currentHasher is StructSerializer)
            {
                hashFunction = this.hasherProvider.NewHasher();
            }

            if (ionValue.CurrentType == IonType.Struct)
            {
                this.currentHasher = new StructSerializer(hashFunction, this.Depth(), this.hasherProvider);
            }
            else
            {
                this.currentHasher = new Serializer(hashFunction, this.Depth());
            }

            this.hasherStack.Push(this.currentHasher);
            this.currentHasher.StepIn(ionValue);
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
