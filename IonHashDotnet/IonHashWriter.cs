namespace IonHashDotnet
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using IonDotnet;

    internal class IonHashWriter : IIonHashWriter, IIonHashValue
    {
        private readonly IIonWriter writer;
        private readonly Hasher hasher;

        internal IonHashWriter(IIonWriter writer, IIonHasherProvider hasherProvider)
        {
            this.writer = writer;
            this.hasher = new Hasher(hasherProvider);
            this.Annotations = new List<SymbolToken>();
        }

        // implements IIonHashValue
        public IList<SymbolToken> Annotations
        {
            get;
            private set;
        }

        public string FieldName
        {
            get;
            private set;
        }

        public bool IsNull
        {
            get;
            private set;
        }

        public IonType Type
        {
            get;
            private set;
        }

        public dynamic Value
        {
            get;
            private set;
        }

        // implements IIonWriter
        public ISymbolTable SymbolTable
        {
            get { return this.writer.SymbolTable; }
        }

        public bool IsInStruct
        {
            get { return this.writer.IsInStruct; }
        }

        // implements IIonHashWriter
        public byte[] Digest()
        {
            return this.hasher.Digest();
        }

        public void AddTypeAnnotation(string annotation)
        {
            this.AddTypeAnnotationSymbol(new SymbolToken(annotation, SymbolToken.UnknownSid));
            this.writer.AddTypeAnnotation(annotation);
        }

        public void AddTypeAnnotationSymbol(SymbolToken annotation)
        {
            this.Annotations.Add(annotation);
            this.writer.AddTypeAnnotationSymbol(annotation);
        }

        public void ClearTypeAnnotations()
        {
            this.Annotations.Clear();
            this.writer.ClearTypeAnnotations();
        }

        public void Dispose()
        {
            this.writer.Dispose();
        }

        public void Finish()
        {
            this.writer.Finish();
        }

        public void Flush()
        {
            this.writer.Flush();
        }

        public void SetFieldName(string name)
        {
            this.FieldName = name;
            this.writer.SetFieldName(name);
        }

        public void SetFieldNameSymbol(SymbolToken symbol)
        {
            this.FieldName = symbol.Text;
            this.writer.SetFieldNameSymbol(symbol);
        }

        public void SetTypeAnnotations(IEnumerable<string> annots)
        {
            this.Annotations.Clear();
            foreach (var annotation in annots)
            {
                this.AddTypeAnnotationSymbol(new SymbolToken(annotation, SymbolToken.UnknownSid));
            }

            this.writer.SetTypeAnnotations(annots);
        }

        public void StepIn(IonType type)
        {
            this.Type = type;
            this.Value = null;
            this.IsNull = false;
            this.hasher.StepIn(this);
            this.writer.StepIn(type);
            this.FieldName = default;
            this.Annotations.Clear();
        }

        public void StepOut()
        {
            this.hasher.StepOut();
            this.writer.StepOut();
        }

        public void WriteBlob(ReadOnlySpan<byte> value)
        {
            byte[] byteList = value.ToArray();
            this.HashScalar(IonType.Blob, byteList);
            this.writer.WriteBlob(value);
        }

        public void WriteBool(bool value)
        {
            this.HashScalar(IonType.Bool, value);
            this.writer.WriteBool(value);
        }

        public void WriteClob(ReadOnlySpan<byte> value)
        {
            byte[] byteList = value.ToArray();
            this.HashScalar(IonType.Clob, byteList);
            this.writer.WriteClob(value);
        }

        public void WriteDecimal(decimal value)
        {
            this.HashScalar(IonType.Decimal, value);
            this.writer.WriteDecimal(value);
        }

        public void WriteDecimal(BigDecimal value)
        {
            this.HashScalar(IonType.Decimal, value);
            this.writer.WriteDecimal(value);
        }

        public void WriteFloat(double value)
        {
            this.HashScalar(IonType.Float, value);
            this.writer.WriteFloat(value);
        }

        public void WriteInt(long value)
        {
            this.HashScalar(IonType.Int, value);
            this.writer.WriteInt(value);
        }

        public void WriteInt(BigInteger value)
        {
            this.HashScalar(IonType.Int, value);
            this.writer.WriteInt(value);
        }

        public void WriteNull()
        {
            this.HashScalar(IonType.Null, null);
            this.writer.WriteNull();
        }

        public void WriteNull(IonType type)
        {
            this.HashScalar(type, null);
            this.writer.WriteNull(type);
        }

        public void WriteString(string value)
        {
            this.HashScalar(IonType.String, value);
            this.writer.WriteString(value);
        }

        public void WriteSymbol(string symbol)
        {
            this.HashScalar(IonType.Symbol, symbol);
            this.writer.WriteSymbol(symbol);
        }

        public void WriteSymbolToken(SymbolToken symbolToken)
        {
            this.HashScalar(IonType.Symbol, symbolToken.Text);
            this.writer.WriteSymbolToken(symbolToken);
        }

        public void WriteTimestamp(Timestamp value)
        {
            this.HashScalar(IonType.Timestamp, value);
            this.writer.WriteTimestamp(value);
        }

        public void WriteValue(IIonReader reader)
        {
            this.InternalWriteValue(reader);
        }

        public void WriteValues(IIonReader reader)
        {
            this.InternalWriteValues(reader);
        }

        private void InternalWriteValue(IIonReader reader, int depth = 0)
        {
            IonType type = reader.CurrentType;
            if (type == IonType.None)
            {
                return;
            }

            if (depth > 0)
            {
                string fieldName = reader.CurrentFieldName;
                if (fieldName != null)
                {
                    this.SetFieldName(fieldName);
                }
            }

            foreach (var annotation in reader.GetTypeAnnotations())
            {
                this.AddTypeAnnotationSymbol(annotation);
            }

            if (reader.CurrentIsNull)
            {
                this.WriteNull(type);
            }
            else
            {
                switch (type)
                {
                    case IonType.Bool:
                        this.WriteBool(reader.BoolValue());
                        break;
                    case IonType.Int:
                        this.WriteInt(reader.BigIntegerValue());
                        break;
                    case IonType.Float:
                        this.WriteFloat(reader.DoubleValue());
                        break;
                    case IonType.Decimal:
                        this.WriteDecimal(reader.DecimalValue());
                        break;
                    case IonType.Timestamp:
                        this.WriteTimestamp(reader.TimestampValue());
                        break;
                    case IonType.Symbol:
                        this.WriteSymbol(reader.StringValue());
                        break;
                    case IonType.String:
                        this.WriteString(reader.StringValue());
                        break;
                    case IonType.Clob:
                        this.WriteClob(reader.NewByteArray());
                        break;
                    case IonType.Blob:
                        this.WriteBlob(reader.NewByteArray());
                        break;
                    case IonType.List:
                        this.StepIn(IonType.List);
                        break;
                    case IonType.Sexp:
                        this.StepIn(IonType.Sexp);
                        break;
                    case IonType.Struct:
                        this.StepIn(IonType.Struct);
                        break;
                    default:
                        throw new InvalidOperationException("Unexpected type '" + type + "'");
                }

                if (type.IsContainer())
                {
                    reader.StepIn();
                    this.InternalWriteValues(reader, depth + 1);
                    this.StepOut();
                    reader.StepOut();
                }
            }
        }

        private void InternalWriteValues(IIonReader reader, int depth = 0)
        {
            IonType type = reader.CurrentType;
            if (type == IonType.None)
            {
                type = reader.MoveNext();
            }

            while (type != IonType.None)
            {
                this.InternalWriteValue(reader, depth);
                type = reader.MoveNext();
            }
        }

        private void HashScalar(IonType type, dynamic value)
        {
            this.Type = type;
            this.Value = value;
            this.IsNull = value == null;
            this.hasher.Scalar(this);
            this.FieldName = default;
            this.Annotations.Clear();
        }
    }
}
