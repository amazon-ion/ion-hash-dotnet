namespace IonHashDotnet
{
    public interface IIonHasher
    {
        void Update(byte[] bytes);

        byte[] Digest();
    }
}
