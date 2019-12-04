namespace Amazon.IonHash
{
    using IonDotnet;

    public class IonHashReaderBuilder
    {
        private IIonHasherProvider hasherProvider;
        private IIonReader reader;

        // no public constructor
        private IonHashReaderBuilder()
        {
        }

        public static IonHashReaderBuilder Standard()
        {
            return new IonHashReaderBuilder();
        }

        public IonHashReaderBuilder WithReader(IIonReader reader)
        {
            this.reader = reader;
            return this;
        }

        public IonHashReaderBuilder WithHasherProvider(IIonHasherProvider hasherProvider)
        {
            this.hasherProvider = hasherProvider;
            return this;
        }

        public IIonHashReader Build()
        {
            return new IonHashReader(this.reader, this.hasherProvider);
        }
    }
}
