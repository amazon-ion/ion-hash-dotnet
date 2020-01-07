namespace IonHashDotnet
{
    using System;
    using IonDotnet;

    public class IonHashWriterBuilder
    {
        private IIonHasherProvider hasherProvider;
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

        public IonHashWriterBuilder WithHasherProvider(IIonHasherProvider hasherProvider)
        {
            this.hasherProvider = hasherProvider;
            return this;
        }

        public IIonHashWriter Build()
        {
            if (this.hasherProvider == null || this.writer == null)
            {
                throw new ArgumentNullException("The Writer and HasherProvider must not be null");
            }

            return new IonHashWriter(this.writer, this.hasherProvider);
        }
    }
}
