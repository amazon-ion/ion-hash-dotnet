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
    using System.Numerics;
    using Amazon.IonDotnet;

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

        public dynamic CurrentValue
        {
            get;
            private set;
        }

        public IonType CurrentType
        {
            get;
            private set;
        }

        public string CurrentFieldName
        {
            get;
            private set;
        }

        public SymbolToken CurrentFieldNameSymbol
        {
            get;
            private set;
        }

        public bool CurrentIsNull
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
            this.CurrentFieldName = name;
            this.writer.SetFieldName(name);
        }

        public void SetFieldNameSymbol(SymbolToken symbol)
        {
            this.CurrentFieldName = symbol.Text;
            this.CurrentFieldNameSymbol = symbol;
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
            this.CurrentType = type;
            this.CurrentValue = null;
            this.CurrentIsNull = false;
            this.hasher.StepIn(this, IsInStruct);
            this.writer.StepIn(type);
            this.CurrentFieldName = default;
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
            this.HashScalar(IonType.Symbol, new SymbolToken(symbol, SymbolToken.UnknownSid));
            this.writer.WriteSymbol(symbol);
        }

        public void WriteSymbolToken(SymbolToken symbolToken)
        {
            this.HashScalar(IonType.Symbol, symbolToken);
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

            if (depth > 0 && IsInStruct)
            {
                this.SetFieldNameSymbol(reader.GetFieldNameSymbol());
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
                        this.WriteSymbolToken(reader.SymbolValue());
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
            this.CurrentType = type;
            this.CurrentValue = value;
            this.CurrentIsNull = value == null;
            this.hasher.Scalar(this, IsInStruct);
            this.CurrentFieldName = default;
            this.CurrentFieldNameSymbol = default;
            this.Annotations.Clear();
        }
    }
}
