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

namespace Amazon.IonHashDotnet.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Amazon.IonDotnet;
    using Amazon.IonDotnet.Builders;
    using Amazon.IonDotnet.Tree;
    using Amazon.IonDotnet.Tree.Impl;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class IonHashTest
    {
        [DataTestMethod]
        [IonHashDataSource]
        public void RunBinaryTest(
            string testName,
            IIonValue testCase,
            IIonValue expectedHashLog,
            TestIonHasherProvider hasherProvider
        )
        {
            RunTest(testName, testCase, expectedHashLog, hasherProvider, new BinaryTest());
        }

        [DataTestMethod]
        [IonHashDataSource]
        public void RunDomTest(
            string testName,
            IIonValue testCase,
            IIonValue expectedHashLog,
            TestIonHasherProvider hasherProvider
        )
        {
            RunTest(testName, testCase, expectedHashLog, hasherProvider, new DomTest());
        }

        [DataTestMethod]
        [IonHashDataSource]
        public void RunTextTest(
            string testName,
            IIonValue testCase,
            IIonValue expectedHashLog,
            TestIonHasherProvider hasherProvider
        )
        {
            RunTest(testName, testCase, expectedHashLog, hasherProvider, new TextTest());
        }

        [DataTestMethod]
        [IonHashDataSource]
        public void RunTextNoStepInTest(
            string testName,
            IIonValue testCase,
            IIonValue expectedHashLog,
            TestIonHasherProvider hasherProvider
        )
        {
            RunTest(testName, testCase, expectedHashLog, hasherProvider, new TextNoStepInTest());
        }

        [DataTestMethod]
        [IonHashDataSource]
        public void RunWriterTest(
            string testName,
            IIonValue testCase,
            IIonValue expectedHashLog,
            TestIonHasherProvider hasherProvider
        )
        {
            RunTest(testName, testCase, expectedHashLog, hasherProvider, new WriterTest());
        }

        private void RunTest(
            string testName,
            IIonValue testCase,
            IIonValue expectedHashLog,
            TestIonHasherProvider hasherProvider,
            IonHashTester testObject
        )
        {
            if (expectedHashLog == null)
            {
                return;
            }

            IIonValue ionValue = testCase.GetField("ion");
            IIonValue ionBinary = testCase.GetField("10n");
            if (ionValue != null && ionBinary != null)
            {
                throw new Exception("Test must not define both 'ion' and '10n' fields");
            }

            var reader = ionValue == null
                ? testObject.GetIonReader(ContainerToBytes(ionBinary))
                : testObject.GetIonReader(ionValue);

            testObject.Traverse(reader, hasherProvider);

            IIonValue actualHashLog = testObject.GetHashLog();
            IIonValue actualHashLogFiltered = FilterHashLog(actualHashLog, expectedHashLog);
            Assert.AreEqual(HashLogToString(expectedHashLog), HashLogToString(actualHashLogFiltered));
        }

        internal static byte[] ContainerToBytes(IIonValue container)
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(new byte[]{0xE0, 0x01, 0x00, 0xEA});
            var bytesEnumerator = container.GetEnumerator();
            while (bytesEnumerator.MoveNext())
            {
                bytes.Add((byte)bytesEnumerator.Current.IntValue);
            }

            return bytes.ToArray();
        }

        private static IIonValue FilterHashLog(IIonValue actualHashLog, IIonValue expectedHashLog)
        {
            HashSet<string> methodCalls = new HashSet<string>();
            IEnumerator<IIonValue> enumerator = expectedHashLog.GetEnumerator();
            while (enumerator.MoveNext())
            {
                IIonValue v = enumerator.Current;
                foreach (SymbolToken annotation in v.GetTypeAnnotationSymbols())
                {
                    methodCalls.Add(annotation.Text);
                }
            }

            IValueFactory valueFactory = new ValueFactory();
            IIonValue result = valueFactory.NewEmptySexp();
            if (methodCalls.Count == 1 && methodCalls.Contains("final_digest"))
            {
                IIonValue finalDigest = actualHashLog.GetElementAt(actualHashLog.Count - 1);
                finalDigest.ClearAnnotations();
                finalDigest.AddTypeAnnotation("final_digest");
                result.Add(finalDigest);
            }
            else
            {
                enumerator = actualHashLog.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    IIonValue v = enumerator.Current;
                    String methodCall = v.GetTypeAnnotationSymbols().ElementAt(0).Text;
                    if (methodCalls.Contains(methodCall))
                    {
                        result.Add(v);
                    }
                }
            }

            return result;
        }

        private static string HashLogToString(IIonValue hashLog)
        {
            bool multipleEntries = hashLog.Count > 1;
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            IEnumerator<IIonValue> enumerator = hashLog.GetEnumerator();
            while (enumerator.MoveNext())
            {
                IIonValue hashCall = enumerator.Current;
                if (multipleEntries)
                {
                    sb.Append("\n  ");
                }

                foreach (SymbolToken annotation in hashCall.GetTypeAnnotationSymbols())
                {
                    sb.Append(annotation.Text).Append("::");
                }

                sb.Append("(");
                int cnt = 0;
                IEnumerator<IIonValue> bytesEnum = hashCall.GetEnumerator();
                while (bytesEnum.MoveNext())
                {
                    IIonValue i = bytesEnum.Current;
                    if (cnt++ > 0)
                    {
                        sb.Append(" ");
                    }

                    sb.Append(i.IntValue.ToString("x2"));
                }

                sb.Append(")");
            }

            if (multipleEntries)
            {
                sb.Append("\n");
            }

            sb.Append(")");
            return sb.ToString();
        }

        internal abstract class IonHashTester
        {
            private protected TestIonHasherProvider hasherProvider;

            internal virtual IIonReader GetIonReader(byte[] ionBinary)
            {
                return IonReaderBuilder.Build(ionBinary);
            }

            internal virtual IIonReader GetIonReader(IIonValue ionValue)
            {
                return IonReaderBuilder.Build(ionValue);
            }

            internal virtual void Traverse(IIonReader reader, TestIonHasherProvider hasherProvider)
            {
                this.hasherProvider = hasherProvider;
                IIonHashReader ihr = new IonHashReader(reader, hasherProvider);
                Traverse(ihr);
                ihr.Digest();
            }

            internal virtual void Traverse(IIonHashReader reader)
            {
                IonType iType;
                while ((iType = reader.MoveNext()) != IonType.None)
                {
                    if (!reader.CurrentIsNull && IonTypeExtensions.IsContainer(iType))
                    {
                        reader.StepIn();
                        Traverse(reader);
                        reader.StepOut();
                    }
                }
            }

            internal virtual IIonValue GetHashLog()
            {
                return this.hasherProvider.GetHashLog();
            }
        }

        internal class BinaryTest : IonHashTester
        {
            internal override IIonReader GetIonReader(IIonValue ionValue)
            {
                MemoryStream ms = new MemoryStream();
                IIonWriter writer = IonBinaryWriterBuilder.Build(ms);
                IIonReader reader = IonReaderBuilder.Build(ionValue.ToPrettyString());
                writer.WriteValues(reader);
                writer.Flush();
                return IonReaderBuilder.Build(ms.ToArray());
            }
        }

        internal class DomTest : IonHashTester { }

        internal class TextTest : IonHashTester
        {
            internal override IIonReader GetIonReader(IIonValue ionValue)
            {
                return IonReaderBuilder.Build(ionValue.ToPrettyString());
            }
        }

        internal class TextNoStepInTest : TextTest
        {
            internal override void Traverse(IIonHashReader reader)
            {
                while (reader.MoveNext() != IonType.None) { }
            }
        }

        internal class WriterTest : IonHashTester
        {
            internal override void Traverse(IIonReader reader, TestIonHasherProvider hasherProvider)
            {
                this.hasherProvider = hasherProvider;
                MemoryStream ms = new MemoryStream();
                StreamWriter sw = new StreamWriter(ms);
                IIonWriter writer = IonTextWriterBuilder.Build(sw);
                IIonHashWriter ihw = new IonHashWriter(writer, this.hasherProvider);
                ihw.WriteValues(reader);
                ihw.Digest();
                ihw.Dispose();
            }
        }
    }
}
