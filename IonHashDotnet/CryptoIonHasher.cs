namespace IonHashDotnet
{
    using System;
    using System.Security.Cryptography;

    internal class CryptoIonHasher : IIonHasher
    {
        private readonly HashAlgorithm hashAlgorithm;

        internal CryptoIonHasher(string algorithm)
        {
            this.hashAlgorithm = HashAlgorithm.Create(algorithm);

            if (this.hashAlgorithm == null)
            {
                throw new ArgumentException("Invalid Algorithm Specified");
            }
        }

        public virtual void Update(byte[] bytes)
        {
            this.hashAlgorithm.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
        }

        public virtual byte[] Digest()
        {
            this.hashAlgorithm.TransformFinalBlock(new byte[0], 0, 0);
            return this.hashAlgorithm.Hash;
        }
    }
}
