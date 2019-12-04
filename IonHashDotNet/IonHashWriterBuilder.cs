namespace Amazon.IonHash
{
    using IonDotnet;

    public class IonHashWriterBuilder
    {
        private IIonHashProvider hasherProvider;
        private IIonWriter writer;

        // no public constructor
        private IonHashWriterBuilder()
        {
        }

        public static IonHashWriterBuilder Standard()
        {
            return new IonHashWriterBuilder();
        }

        public IonHashWriterBuilder WithWriter(IIonWriter writer)
        {
            this.writer = writer;
            return this;
        }

        public IonHashWriterBuilder WithHasherProvider(IIonHashProvider hasherProvider)
        {
            this.hasherProvider = hasherProvider;
            return this;
        }

        public IIonHashWriter Build()
        {
            return new IonHashWriter(this.writer, this.hasherProvider);
        }
    }
}
