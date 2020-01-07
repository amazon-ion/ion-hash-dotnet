namespace IonHashDotnet
{
    using System;
    using IonDotnet;

    public class IonHashReaderBuilder
    {
        private IIonHasherProvider hasherProvider = null;
        private IIonReader reader = null;

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
            if (this.hasherProvider == null || this.reader == null)
            {
                throw new ArgumentNullException("The Reader and HasherProvider must not be null");
            }

            return new IonHashReader(this.reader, this.hasherProvider);
        }
    }
}
