namespace Amazon.IonHash
{
    public interface IIonHasherProvider
    {
        IIonHasher NewHasher();
    }
}
