namespace IonHashDotnet
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

        internal override void Scalar(IIonValue value)
        {
            this.scalarSerializer.HandleFieldName(value.FieldName);
            this.scalarSerializer.Scalar(value);
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

        private class DigestComparer : IComparer<byte[]>
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
