namespace IonHashDotnet
{
    using IonDotnet;

    public interface IIonHashReader : IIonReader
    {
        byte[] Digest();
    }
}
