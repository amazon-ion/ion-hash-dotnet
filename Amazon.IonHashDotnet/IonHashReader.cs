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
    using System.Linq;
    using System.Numerics;
    using Amazon.IonDotnet;

    internal class IonHashReader : IIonHashReader, IIonHashValue
    {
        private readonly IIonReader reader;
        private readonly Hasher hasher;
        private IonType ionType = IonType.None;

        internal IonHashReader(IIonReader reader, IIonHasherProvider hasherProvider)
        {
            this.hasher = new Hasher(hasherProvider);
            this.reader = reader;
        }

        // implements IIonHashValue
        public IList<SymbolToken> Annotations
        {
            get { return this.GetTypeAnnotationSymbols().ToList(); }
        }

        public dynamic CurrentValue
        {
            get { return this.GetIonValue(); }
        }

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

        // implements IIonReader
        public bool IsInStruct
        {
            get { return this.reader.IsInStruct; }
        }

        public int CurrentDepth
        {
            get { return this.reader.CurrentDepth; }
        }

        public SymbolToken CurrentFieldNameSymbol
        {
            get
            {
                return this.GetFieldNameSymbol();
            }
        }

        // implements IIonHashReader
        public byte[] Digest()
        {
            return this.hasher.Digest();
        }

        /// <summary>
        /// Dispose the IIonReader.
        /// </summary>
        public void Dispose()
        {
            this.reader.Dispose();
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

        public string[] GetTypeAnnotations()
        {
            return this.reader.GetTypeAnnotations();
        }

        public IEnumerable<SymbolToken> GetTypeAnnotationSymbols()
        {
            return this.reader.GetTypeAnnotationSymbols();
        }

        public bool HasAnnotation(string annotation)
        {
            return this.reader.HasAnnotation(annotation);
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
                case IonType.Null:
                    return IonType.Null;
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
