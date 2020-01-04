namespace IonHashDotnet
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using IonDotnet;
    using IonDotnet.Systems;

    internal class Serializer
    {
        private readonly int depth;
        private bool hasContainerAnnotation = false;

        internal Serializer(IIonHasher hashFunction, int depth)
        {
            this.HashFunction = hashFunction;
            this.depth = depth;
        }

        internal IIonHasher HashFunction { get; private set; }

        internal virtual void Scalar(IIonValue ionValue)
        {
            this.HandleAnnotationsBegin(ionValue);
            this.BeginMarker();
            byte[] scalarBytes = this.GetBytes(ionValue.Type, ionValue.Value, ionValue.IsNull);
            (byte tq, byte[] representation) = this.ScalarOrNullSplitParts(
                ionValue.Type,
                ionValue.IsNull,
                scalarBytes);

            this.Update(new byte[] { tq });
            if (representation.Length > 0)
            {
                this.Update(Escape(representation));
            }

            this.EndMarker();
            this.HandleAnnotationsEnd(ionValue);
        }

        internal void StepIn(IIonValue ionValue)
        {
            this.HandleFieldName(ionValue.FieldName);
            this.HandleAnnotationsBegin(ionValue, true);
            this.BeginMarker();
            byte tq = TQ(ionValue);
            if (ionValue.IsNull)
            {
                tq |= 0x0F;
            }

            this.Update(new byte[] { tq });
        }

        internal virtual void StepOut()
        {
            this.EndMarker();
            this.HandleAnnotationsEnd(null, true);
        }

        internal byte[] Digest()
        {
            this.HashFunction.TransformFinalBlock(new byte[0], 0, 0);
            return this.HashFunction.Hash;
        }

        internal void HandleFieldName(string fieldName)
        {
            // the "!= null" condition allows the empty symbol to be written
            if (fieldName != null && this.depth > 0)
            {
                this.WriteSymbol(fieldName);
            }
        }

        private protected static byte[] Escape(byte[] bytes)
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

        private protected void Update(byte[] bytes)
        {
            this.HashFunction.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
        }

        private protected void BeginMarker()
        {
            this.HashFunction.TransformBlock(
                Constants.BeginMarker,
                0,
                Constants.BeginMarker.Length,
                Constants.BeginMarker,
                0);
        }

        private protected void EndMarker()
        {
            this.HashFunction.TransformBlock(
                Constants.EndMarker,
                0,
                Constants.EndMarker.Length,
                Constants.EndMarker,
                0);
        }

        private static void Serializers(IonType type, dynamic value, IIonWriter writer)
        {
            switch (type)
            {
                case IonType.Bool:
                    writer.WriteBool(value);
                    break;
                case IonType.Blob:
                    writer.WriteBlob(value);
                    break;
                case IonType.Clob:
                    writer.WriteClob(value);
                    break;
                case IonType.Decimal:
                    writer.WriteDecimal(value);
                    break;
                case IonType.Float:
                    writer.WriteFloat(value);
                    break;
                case IonType.Int:
                    writer.WriteInt(value);
                    break;
                case IonType.String:
                    writer.WriteString(value);
                    break;
                case IonType.Symbol:
                    writer.WriteSymbol(value);
                    break;
                case IonType.Timestamp:
                    writer.WriteTimestamp(value);
                    break;
                case IonType.Null:
                    writer.WriteNull(IonType.Null);
                    break;
                default:
                    throw new InvalidOperationException("Unexpected type '" + type + "'");
            }
        }

        private static byte TQ(IIonValue ionValue)
        {
            byte typeCode = (byte)ionValue.Type.GetTypeCode();
            return (byte)(typeCode << 4);
        }

        private void HandleAnnotationsBegin(IIonValue ionValue, bool isContainer = false)
        {
            IList<SymbolToken> annotations = ionValue.Annotations;
            if (annotations.Count > 0)
            {
                this.BeginMarker();
                this.Update(Constants.TqAnnotatedValue);
                foreach (var annotation in annotations)
                {
                    this.WriteSymbol(annotation.Text);
                }

                if (isContainer)
                {
                    this.hasContainerAnnotation = true;
                }
            }
        }

        private void HandleAnnotationsEnd(IIonValue ionValue, bool isContainer = false)
        {
            if ((ionValue != null && ionValue.Annotations.Count > 0) || (isContainer && this.hasContainerAnnotation))
            {
                this.EndMarker();
                if (isContainer)
                {
                    this.hasContainerAnnotation = false;
                }
            }
        }

        private void WriteSymbol(string token)
        {
            this.BeginMarker();
            byte[] scalarBytes = this.GetBytes(IonType.Symbol, token, false);
            (byte tq, byte[] representation) = this.ScalarOrNullSplitParts(IonType.Symbol, false, scalarBytes);

            this.Update(new byte[] { tq });
            if (representation.Length > 0)
            {
                this.Update(Escape(representation));
            }

            this.EndMarker();
        }

        private byte[] GetBytes(IonType type, dynamic value, bool isNull)
        {
            if (isNull)
            {
                byte typeCode = (byte)type.GetTypeCode();
                return new byte[] { (byte)(typeCode << 4 | 0x0F) };
            }
            else
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (IIonWriter writer = IonBinaryWriterBuilder.Build(stream))
                    {
                        Serializers(type, value, writer);
                    }

                    return stream.ToArray().Skip(4).ToArray();
                }
            }
        }

        private int GetLengthLength(byte[] bytes)
        {
            if ((bytes[0] & 0x0F) == 0x0E)
            {
                // read subsequent byte(s) as the "length" field
                for (var i = 0; i < bytes.Length; i++)
                {
                    if ((bytes[i] & 0x80) != 0)
                    {
                        return i;
                    }
                }

                throw new IonHashException("Problem while reading VarUInt!");
            }

            return 0;
        }

        private (byte, byte[]) ScalarOrNullSplitParts(IonType type, bool isNull, byte[] bytes)
        {
            int offset = this.GetLengthLength(bytes) + 1;

            // the representation is everything after TL (first byte) and length
            byte[] representation = bytes.Skip(offset).ToArray();
            byte tq = bytes[0];

            if (type == IonType.Symbol)
            {
                // symbols are serialized as strings; use the correct TQ:
                tq = 0x70;
                if (isNull)
                {
                    tq |= 0x0F;
                }
            }

            // not a bool, symbol, or null value
            if (type != IonType.Bool && type != IonType.Symbol && (tq & 0x0F) != 0x0F)
            {
                // zero - out the L nibble
                tq &= 0xF0;
            }

            return (tq, representation);
        }
    }
}
