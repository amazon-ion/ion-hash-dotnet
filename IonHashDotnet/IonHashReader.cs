namespace IonHashDotnet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using IonDotnet;

    internal class IonHashReader : IIonHashReader, IIonValue
    {
        private readonly IIonReader reader;
        private readonly Hasher hasher;
        private IonType ionType = IonType.None;

        internal IonHashReader(IIonReader reader, IIonHasherProvider hasherProvider)
        {
            this.hasher = new Hasher(hasherProvider);
            this.reader = reader;
        }

        // implements IIonReader
        public IonType CurrentType
        {
            get { return this.reader.CurrentType; }
        }

        public string CurrentFieldName
        {
            get { return this.reader.CurrentFieldName; }
        }

        public bool CurrentIsNull
        {
            get { return this.reader.CurrentIsNull; }
        }

        public bool IsInStruct
        {
            get { return this.reader.IsInStruct; }
        }

        public int CurrentDepth
        {
            get { return this.reader.CurrentDepth; }
        }

        // implements IIonValue
        public IList<SymbolToken> Annotations
        {
            get { return this.GetTypeAnnotations().ToList(); }
        }

        public string FieldName
        {
            get { return this.CurrentFieldName; }
        }

        public bool IsNull
        {
            get { return this.CurrentIsNull;  }
        }

        public IonType Type
        {
            get { return this.CurrentType; }
        }

        public dynamic Value
        {
            get { return this.GetIonValue(); }
        }

        // implements IIonHashReader
        public byte[] Digest()
        {
            return this.hasher.Digest();
        }

        public void StepIn()
        {
            this.hasher.StepIn(this);
            this.reader.StepIn();
            this.ionType = IonType.None;
        }

        public void StepOut()
        {
            this.Traverse();
            this.reader.StepOut();
            this.hasher.StepOut();
        }

        public IonType MoveNext()
        {
            if (this.ionType != IonType.None && this.ionType.IsContainer())
            {
                if (this.CurrentIsNull)
                {
                    this.hasher.Scalar(this);
                }
                else
                {
                    // caller is nexting past a container;  perform deep traversal to ensure hashing correctness
                    this.StepIn();
                    this.Traverse();
                    this.StepOut();
                }
            }

            if (this.ionType != IonType.None && this.ionType.IsScalar())
            {
                this.hasher.Scalar(this);
            }

            this.ionType = this.reader.MoveNext();
            return this.ionType;
        }

        // implements IIonReader
        public ISymbolTable GetSymbolTable()
        {
            return this.reader.GetSymbolTable();
        }

        public bool BoolValue()
        {
            return this.reader.BoolValue();
        }

        public BigInteger BigIntegerValue()
        {
            return this.reader.BigIntegerValue();
        }

        public int IntValue()
        {
            return this.reader.IntValue();
        }

        public long LongValue()
        {
            return this.reader.LongValue();
        }

        public double DoubleValue()
        {
            return this.reader.DoubleValue();
        }

        public BigDecimal DecimalValue()
        {
            return this.reader.DecimalValue();
        }

        public Timestamp TimestampValue()
        {
            return this.reader.TimestampValue();
        }

        public string StringValue()
        {
            return this.reader.StringValue();
        }

        public SymbolToken SymbolValue()
        {
            return this.reader.SymbolValue();
        }

        public IntegerSize GetIntegerSize()
        {
            return this.reader.GetIntegerSize();
        }

        public SymbolToken GetFieldNameSymbol()
        {
            return this.reader.GetFieldNameSymbol();
        }

        public int GetBytes(Span<byte> buffer)
        {
            return this.reader.GetBytes(buffer);
        }

        public IEnumerable<SymbolToken> GetTypeAnnotations()
        {
            return this.reader.GetTypeAnnotations();
        }

        public byte[] NewByteArray()
        {
            return this.reader.NewByteArray();
        }

        public int GetLobByteSize()
        {
            return this.reader.GetLobByteSize();
        }

        private dynamic GetIonValue()
        {
            switch (this.CurrentType)
            {
                case IonType.Bool:
                    return this.BoolValue();
                case IonType.Blob:
                    return this.NewByteArray();
                case IonType.Clob:
                    return this.NewByteArray();
                case IonType.Decimal:
                    return this.DecimalValue();
                case IonType.Float:
                    return this.DoubleValue();
                case IonType.Int:
                    return this.BigIntegerValue();
                case IonType.String:
                    return this.StringValue();
                case IonType.Symbol:
                    return this.SymbolValue();
                case IonType.Timestamp:
                    return this.TimestampValue();
                default:
                    throw new InvalidOperationException("Unexpected type '" + this.CurrentType + "'");
            }
        }

        private void Traverse()
        {
            while ((this.ionType = this.MoveNext()) != IonType.None)
            {
                if (this.ionType.IsContainer() && !this.CurrentIsNull)
                {
                    this.StepIn();
                    this.Traverse();
                    this.StepOut();
                }
            }
        }
    }
}
