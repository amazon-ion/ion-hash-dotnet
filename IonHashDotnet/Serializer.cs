namespace IonHashDotnet
{
    using System;
    using System.Collections.Generic;
    using IonDotnet;

    internal class Serializer
    {
        private bool hasContainerAnnotation = false;

        internal Serializer(IIonHasher hashFunction, int depth)
        {
            this.HashFunction = hashFunction;
            this.Depth = depth;
        }

        internal int Depth { get; }

        internal IIonHasher HashFunction { get; private set; }

        internal void Scalar(IIonValue ionValue)
        {
            this.HandleAnnotationsBegin(ionValue);
            this.BeginMarker();
            byte[] scalarBytes = this.GetBytes(ionValue.Type, ionValue.Value, ionValue.IsNull);
            byte[][] tqAndRepresentation = this.ScalarOrNullSplitParts(ionValue.Type, ionValue.IsNull, scalarBytes);
            this.Update(tqAndRepresentation[0]);
            if (tqAndRepresentation[1].Length > 0)
            {
                this.Update(Utils.Escape(tqAndRepresentation[1]));
            }

            this.EndMarker();
            this.HandleAnnotationsEnd(ionValue);
        }

        internal void StepIn(IIonValue ionValue)
        {
            this.HandleFieldName(ionValue.FieldName);
            this.HandleAnnotationsBegin(ionValue, true);
            this.BeginMarker();
            //var tq = tq()
        }

        internal void StepOut()
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
            if (fieldName != null && this.Depth > 0)
            {
                this.WriteSymbol(fieldName);
            }
        }

        protected void Update(byte[] bytes)
        {
            this.HashFunction.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
        }

        protected void BeginMarker()
        {
            this.HashFunction.TransformBlock(
                Constants.BeginMarker,
                0,
                Constants.BeginMarker.Length,
                Constants.BeginMarker,
                0);
        }

        protected void EndMarker()
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
            var scalarBytes = this.GetBytes(IonType.Symbol, token, false);
        }

        private byte[] GetBytes(IonType type, dynamic value, bool isNull)
        {
            throw new NotImplementedException();
        }

        private int GetLengthLength(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        private byte[][] ScalarOrNullSplitParts(IonType type, bool isNull, byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
