namespace Amazon.IonHash
{
    using System;
    using System.Collections.Generic;
    using IonDotnet;

    internal class Hasher
    {
        private IIonHashProvider hasherProvider;
        private Serializer currentHasher;
        private Stack<Serializer> hasherStack;

        internal Hasher(IIonHashProvider hasherProvider)
        {
            this.hasherProvider = hasherProvider;
            this.currentHasher = new Serializer(hasherProvider.NewHasher(), 0);
            this.hasherStack.Push(this.currentHasher);
        }

        internal void Scalar(IIonValue ionValue)
        {
            this.currentHasher.Scalar(ionValue);
        }

        internal void StepIn(IIonValue ionValue)
        {
            IIonHasher hashFunction = this.currentHasher.HashFunction;
            if (this.currentHasher.GetType() == typeof(StructSerializer))
            {
                hashFunction = this.hasherProvider.NewHasher();
            }

            if (ionValue.Type == IonType.Struct)
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

            if (this.currentHasher.GetType() == typeof(StructSerializer))
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
