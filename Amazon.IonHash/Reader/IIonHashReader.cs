namespace Amazon.IonHash
{
    using IonDotnet;

    public interface IIonHashReader : IIonReader
    {
        byte[] Digest();
    }
}
