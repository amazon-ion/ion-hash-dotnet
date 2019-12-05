namespace IonHashDotnet
{
    public class CryptoIonHasherProvider : IIonHasherProvider
    {
        private readonly string algorithm;

        public CryptoIonHasherProvider(string algorithm)
        {
            this.algorithm = algorithm;
        }

        public IIonHasher NewHasher()
        {
            return new CryptoIonHasher(this.algorithm);
        }
    }
}
