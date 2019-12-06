namespace IonHashDotnet
{
    using System.Collections.Generic;

    internal static class Utils
    {
        internal static byte[] Escape(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = bytes[i];
                if (b == Constants.BeginMarkerByte || b == Constants.EndMarkerByte || b == Constants.EscapeByte)
                {
                    // found a byte that needs to be escaped; build a new byte array that
                    // escapes that byte as well as any others
                    List<byte> escapedBytes = new List<byte>();

                    for (int j = 0; j < bytes.Length; j++)
                    {
                        byte c = bytes[j];
                        if (c == Constants.BeginMarkerByte || c == Constants.EndMarkerByte || c == Constants.EscapeByte)
                        {
                            escapedBytes.Add(Constants.EscapeByte);
                        }

                        escapedBytes.Add(c);
                    }

                    return escapedBytes.ToArray();
                }
            }

            return bytes;
        }
    }
}
