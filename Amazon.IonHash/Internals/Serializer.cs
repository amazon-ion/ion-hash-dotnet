namespace Amazon.IonHash
{
    using System;
    using IonDotnet;
    using IonDotnet.Tree;

    internal class Serializer
    {
        internal IIonHasher HashFunction;

        private bool hashContainerAnnotation;

        internal Serializer(IIonHasher hashFunction, int depth)
        {
        }

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
                    writer.WriteNull();
                    break;
                default:
                    throw new SystemException("Unexpected type '" + type + "'");
            }
        }

        private void HandleAnnotationsBegin(IIonValue ionValue, bool isContainer = false)
        {
            throw new NotImplementedException();
        }

        private void HandleAnnotationsEnd(IIonValue ionValue, bool isContainer = false)
        {
            throw new NotImplementedException();
        }

        private void WriteSymbol(string token)
        {
            throw new NotImplementedException();
        }

        private void GetBytes(IonType type, dynamic value, bool isNull)
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
