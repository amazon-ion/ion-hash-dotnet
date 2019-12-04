namespace IonHashDotnet
{
    public interface IIonHasher
    {
        byte[] Hash
        {
            get;
        }

        int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset);

        byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount);
    }
}
