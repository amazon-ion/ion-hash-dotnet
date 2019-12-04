namespace IonHashDotnet
{
    using IonDotnet;

    public interface IIonHashWriter : IIonWriter
    {
        byte[] Digest();
    }
}
