﻿namespace IonHashDotnet
{
    using System;
    using System.Collections.Generic;
<<<<<<< HEAD
    using System.IO;
    using System.Linq;
=======
>>>>>>> Private functions handling annotations
    using IonDotnet;
    using IonDotnet.Systems;

    internal class Serializer
    {
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
        private bool hasContainerAnnotation;
=======
        private bool hasContainerAnnotation = false;
>>>>>>> initial commit
=======

        private bool hasContainerAnnotation;
>>>>>>> Private functions handling annotations
=======
        private bool hasContainerAnnotation = false;
>>>>>>> initial commit

        internal Serializer(IIonHasher hashFunction, int depth)
        {
            this.HashFunction = hashFunction;
            this.Depth = depth;
        }

<<<<<<< HEAD
<<<<<<< HEAD
        internal int Depth { get; }

=======
>>>>>>> Private functions handling annotations
=======
        internal int Depth { get; }

>>>>>>> initial commit
        internal IIonHasher HashFunction { get; private set; }

        internal void Scalar(IIonValue ionValue)
        {
            throw new NotImplementedException();
        }

        internal void StepIn(IIonValue ionValue)
        {
            throw new NotImplementedException();
        }

        internal void StepOut()
        {
            throw new NotImplementedException();
        }

        internal byte[] Digest()
        {
            throw new NotImplementedException();
        }

        internal void HandleFieldName(string fieldName)
        {
            throw new NotImplementedException();
        }

        protected void Update(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        protected void BeginMarker()
        {
            throw new NotImplementedException();
        }

        protected void EndMarker()
        {
            throw new NotImplementedException();
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

        private static byte[] Escape(byte[] bytes)
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
<<<<<<< HEAD
            byte[] scalarBytes = this.GetBytes(IonType.Symbol, token, false);
            (byte tq, byte[] representation) tuple = this.ScalarOrNullSplitParts(IonType.Symbol, false, scalarBytes);

            this.Update(new byte[] { tuple.tq });
            if (tuple.representation.Length > 0)
            {
                this.Update(Escape(tuple.representation));
            }

            this.EndMarker();
=======
            var scalarBytes = this.GetBytes(IonType.Symbol, token, false);
>>>>>>> Private functions handling annotations
        }

        private byte[] GetBytes(IonType type, dynamic value, bool isNull)
        {
            if (isNull)
            {
                // https://github.com/amzn/ion-dotnet/issues/13
                throw new NotImplementedException();
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
                tq &= 0x0F;
            }

            return (tq, representation);
        }
    }
}
