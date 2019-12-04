namespace IonHashDotnet
{
    public class CryptoHasherProvider : IIonHasherProvider
    {
        private readonly string algorithm;

        public CryptoHasherProvider(string algorithm)
        {
            this.algorithm = algorithm;
        }

        public IIonHasher NewHasher()
        {
            return new CryptoIonHasher(this.algorithm);
        }
    }
}
