namespace Amazon.IonHash
{
    using IonDotnet;

    public interface IIonHashWriter : IIonWriter
    {
        byte[] Digest();
    }
}
