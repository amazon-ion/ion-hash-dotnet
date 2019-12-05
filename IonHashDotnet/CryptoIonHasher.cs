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

        public byte[] Hash
        {
            get { return this.hashAlgorithm.Hash; }
        }

        public int TransformBlock(
            byte[] inputBuffer,
            int inputOffset,
            int inputCount,
            byte[] outputBuffer,
            int outputOffset)
        {
            return this.hashAlgorithm.TransformBlock(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            return this.hashAlgorithm.TransformFinalBlock(inputBuffer, inputOffset, inputCount);
        }
    }
}
