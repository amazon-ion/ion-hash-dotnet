namespace IonHashDotnet.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using IonDotnet;
    using IonDotnet.Builders;
    using IonDotnet.Tree;
    using IonDotnet.Tree.Impl;
    using IonHashDotnet;
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

            IIonValue ionText = testCase.GetField("ion");
            IIonValue ionBinary = testCase.GetField("10n");
            if (ionText != null && ionBinary != null)
            {
                throw new Exception("Test must not define both 'ion' and '10n' fields");
            }

            IIonReader reader = ionText != null
                ? testObject.GetIonReader(ionText.ToPrettyString())
                : testObject.GetIonReader(ContainerToBytes(ionBinary));

            testObject.Traverse(reader, hasherProvider);

            IIonValue actualHashLog = testObject.GetHashLog();
            IIonValue actualHashLogFiltered = FilterHashLog(actualHashLog, expectedHashLog);
            Assert.AreEqual(HashLogToString(expectedHashLog), HashLogToString(actualHashLogFiltered));
        }

        private static byte[] ContainerToBytes(IIonValue container)
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
                foreach (SymbolToken annotation in v.GetTypeAnnotations())
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
                    String methodCall = v.GetTypeAnnotations().ElementAt(0).Text;
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

                foreach (SymbolToken annotation in hashCall.GetTypeAnnotations())
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

            internal virtual IIonReader GetIonReader(string ionText)
            {
                return IonReaderBuilder.Build(ionText);
            }

            internal virtual IIonReader GetIonReader(byte[] ionBinary)
            {
                return IonReaderBuilder.Build(ionBinary);
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
            internal override IIonReader GetIonReader(string ionText)
            {
                IValueFactory valueFactory = new ValueFactory();
                MemoryStream ms = new MemoryStream();
                IIonWriter writer = IonBinaryWriterBuilder.Build(ms);
                IIonValue ionString = valueFactory.NewString(ionText);
                ionString.WriteTo(writer);
                writer.Flush();
                return IonReaderBuilder.Build(ms.ToArray());
            }
        }

        internal class DomTest : IonHashTester { }

        internal class TextTest : IonHashTester { }

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
