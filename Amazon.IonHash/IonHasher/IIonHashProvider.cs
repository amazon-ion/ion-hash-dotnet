namespace Amazon.IonHash
{
    public interface IIonHashProvider
    {
        IIonHasher NewHasher();
    }
}