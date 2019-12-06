namespace IonHashDotnet
{
    internal static class Constants
    {
        internal static readonly byte BeginMarkerByte = 0x0B;
        internal static readonly byte EndMarkerByte = 0x0E;
        internal static readonly byte EscapeByte = 0x0C;
        internal static readonly byte[] BeginMarker = { BeginMarkerByte };
        internal static readonly byte[] EndMarker = { EndMarkerByte };

        internal static readonly byte[] TqAnnotatedValue = { 0xE0 };
    }
}
