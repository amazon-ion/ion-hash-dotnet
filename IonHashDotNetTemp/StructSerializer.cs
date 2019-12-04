namespace Amazon.IonHash
{
    using System;

    internal class StructSerializer : Serializer
    {
        private readonly Serializer scalarSerializer;
        private readonly byte[] fieldHashes;

        internal StructSerializer(IIonHasher hashFunction, int depth, IIonHasherProvider hashFunctionProvider)
            : base(hashFunction, depth)
        {
        }

        internal void Scalar(IIonValue value)
        {
            throw new NotImplementedException();
        }

        internal void StepOut()
        {
            throw new NotImplementedException();
        }

        internal void AppendFieldHash(byte[] digest)
        {
            throw new NotImplementedException();
        }
    }
}
