namespace IonHashDotnet
{
    using System;
    using IonDotnet;

    public class IonHashException : IonException
    {
        public IonHashException(Exception inner)
            : base(inner)
        {
        }

        public IonHashException(string message)
            : base(message)
        {
        }

        public IonHashException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
