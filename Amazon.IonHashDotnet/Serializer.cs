/*
 * Copyright 2020 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */

namespace Amazon.IonHashDotnet
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Amazon.IonDotnet;
    using Amazon.IonDotnet.Builders;

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

        internal static byte[] Escape(byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }

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

        internal virtual void Scalar(IIonHashValue ionValue)
        {
            this.HandleAnnotationsBegin(ionValue);
            this.BeginMarker();

            dynamic ionValueValue = ionValue.CurrentIsNull ? null : ionValue.CurrentValue;
            IonType ionType = ionValue.CurrentIsNull ? IonType.None : ionValue.CurrentType;
            byte[] scalarBytes = this.GetBytes(ionValue.CurrentType, ionValueValue, ionValue.CurrentIsNull);
            ionValueValue = ionValue.CurrentType == IonType.Symbol ? ionValueValue : null;
            (byte tq, byte[] representation) = ((byte, byte[]))this.ScalarOrNullSplitParts(
                ionType,
                ionValueValue,
                ionValue.CurrentIsNull,
                scalarBytes);

            this.Update(new byte[] { tq });
            if (representation.Length > 0)
            {
                this.Update(Escape(representation));
            }

            this.EndMarker();
            this.HandleAnnotationsEnd(ionValue);
        }

        internal void StepIn(IIonHashValue ionValue)
        {
            this.HandleFieldName(ionValue);
            this.HandleAnnotationsBegin(ionValue, true);
            this.BeginMarker();
            byte tq = TQ(ionValue);
            if (ionValue.CurrentIsNull)
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
            return this.HashFunction.Digest();
        }

        internal void HandleFieldName(IIonHashValue ionValue)
        {
            if (this.depth > 0 && ionValue.IsInStruct)
            {
                if (ionValue.CurrentFieldNameSymbol.Text == null &&
                    ionValue.CurrentFieldNameSymbol.Sid != 0)
                {
                    throw new UnknownSymbolException(ionValue.CurrentFieldNameSymbol.Sid);
                }

                this.WriteSymbol(ionValue.CurrentFieldNameSymbol.Text);
            }
        }

        private protected void Update(byte[] bytes)
        {
            this.HashFunction.Update(bytes);
        }

        private protected void BeginMarker()
        {
            this.HashFunction.Update(Constants.BeginMarker);
        }

        private protected void EndMarker()
        {
            this.HashFunction.Update(Constants.EndMarker);
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
                    writer.WriteString(value.Text);
                    break;
                case IonType.Timestamp:
                    writer.WriteTimestamp(value);
                    break;
                case IonType.Null:
                    writer.WriteNull(value);
                    break;
                default:
                    throw new InvalidOperationException("Unexpected type '" + type + "'");
            }
        }

        private static byte TQ(IIonHashValue ionValue)
        {
            byte typeCode = (byte)ionValue.CurrentType;
            return (byte)(typeCode << 4);
        }

        private void HandleAnnotationsBegin(IIonHashValue ionValue, bool isContainer = false)
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

        private void HandleAnnotationsEnd(IIonHashValue ionValue, bool isContainer = false)
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
            int sid = token == null ? 0 : SymbolToken.UnknownSid;
            SymbolToken symbolToken = new SymbolToken(token, sid);
            byte[] scalarBytes = this.GetBytes(IonType.Symbol, symbolToken, false);
            (byte tq, byte[] representation) = this.ScalarOrNullSplitParts(IonType.Symbol, symbolToken, false, scalarBytes);

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
                byte typeCode = (byte)type;
                return new byte[] { (byte)(typeCode << 4 | 0x0F) };
            }
            else if (type == IonType.Float && value == 0 && BitConverter.DoubleToInt64Bits(value) >= 0)
            {
                // value is 0.0, not -0.0
                return new byte[] { 0x40 };
            }
            else
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (IIonWriter writer = IonBinaryWriterBuilder.Build(stream, forceFloat64: true))
                    {
                        Serializers(type, value, writer);
                        writer.Finish();
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
                for (var i = 1; i < bytes.Length; i++)
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

        private (byte, byte[]) ScalarOrNullSplitParts(IonType type, SymbolToken? symbolToken, bool isNull, byte[] bytes)
        {
            int offset = this.GetLengthLength(bytes) + 1;

            if (type == IonType.Int && bytes.Length > offset)
            {
                // ignore sign byte prepended by BigInteger.toByteArray() when the magnitude
                // ends at byte boundary (the 'intLength512' test is an example of this)
                if ((bytes[offset] & 0xFF) == 0)
                {
                    offset++;
                }
            }

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
                else if (symbolToken != null && symbolToken.Value.Text == null && symbolToken.Value.Sid == 0)
                {
                    tq = 0x71;
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
