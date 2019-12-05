namespace IonHashDotnet
{
    using System;
    using System.Collections.Generic;
    using IonDotnet;

    internal class Serializer
    {

        private bool hasContainerAnnotation;

        internal Serializer(IIonHasher hashFunction, int depth)
        {
        }

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
