namespace IonHashDotnet.Tests
{
    using System;
    using System.Linq;
    using IonDotnet;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class ReaderCompare
    {
        public static void Compare(IIonReader it1, IIonReader it2)
        {
            while (HasNext(it1, it2))
            {
                IonType t1 = it1.CurrentType;
                IonType t2 = it2.CurrentType;

                if (t1 != t2 && !t1.Equals(t2))
                {
                    Assert.AreEqual(t1, t2, "ion type");
                }

                if (t1 == IonType.None)
                {
                    break;
                }

                if (it1.IsInStruct)
                {
                    CompareFieldNames(it1, it2);
                }

                CompareAnnotations(it1, it2);

                bool isNull = it1.CurrentIsNull;
                Assert.AreEqual(isNull, it2.CurrentIsNull);

                switch (t1)
                {
                    case IonType.Null:
                        Assert.IsTrue(it1.CurrentIsNull);
                        Assert.IsTrue(it2.CurrentIsNull);
                        break;
                    case IonType.Bool:
                    case IonType.Int:
                    case IonType.Float:
                    case IonType.Decimal:
                    case IonType.Timestamp:
                    case IonType.String:
                    case IonType.Symbol:
                    case IonType.Blob:
                    case IonType.Clob:
                        CompareScalars(t1, isNull, it1, it2);
                        break;
                    case IonType.Struct:
                    case IonType.List:
                    case IonType.Sexp:
                        it1.StepIn();
                        it2.StepIn();
                        Compare(it1, it2);
                        it1.StepOut();
                        it2.StepOut();
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }

            Assert.IsFalse(HasNext(it1, it2));
        }

        private static bool HasNext(IIonReader it1, IIonReader it2)
        {
            bool more = it1.MoveNext() != IonType.None;
            Assert.AreEqual(more, it2.MoveNext() != IonType.None, "next result don't match");

            if (!more)
            {
                Assert.AreEqual(IonType.None, it1.MoveNext());
                Assert.AreEqual(IonType.None, it2.MoveNext());
            }

            return more;
        }

        private static void CompareNonNullStrings(string what, string s1, string s2)
        {
            Assert.IsNotNull(s1, what);
            Assert.IsNotNull(s2, what);
            Assert.AreEqual(s1, s2, what);
        }

        private static void CompareFieldNames(IIonReader r1, IIonReader r2)
        {
            SymbolToken tok1 = r1.GetFieldNameSymbol();
            SymbolToken tok2 = r2.GetFieldNameSymbol();
            string fn = tok1.Text;
            Assert.AreEqual(fn, tok2.Text);

            if (fn != null)
            {
                string f1 = r1.CurrentFieldName;
                string f2 = r2.CurrentFieldName;
                CompareNonNullStrings("field name", fn, f1);
                CompareNonNullStrings("field name", fn, f2);
            }
        }

        private static void CompareAnnotations(IIonReader it1, IIonReader it2)
        {
            SymbolToken[] syms1 = it1.GetTypeAnnotations().ToArray();
            SymbolToken[] syms2 = it2.GetTypeAnnotations().ToArray();

            AssertSymbolEquals("annotation", syms1, syms2);
        }

        private static void CompareScalars(IonType t, bool isNull, IIonReader it1, IIonReader it2)
        {
            switch (t)
            {
                case IonType.Bool:
                    {
                        if (!isNull)
                        {
                            Assert.AreEqual(it1.BoolValue(), it2.BoolValue());
                        }

                        break;
                    }

                case IonType.Int:
                    {
                        if (!isNull)
                        {
                            Assert.AreEqual(it1.BigIntegerValue(), it2.BigIntegerValue());
                        }

                        break;
                    }

                case IonType.Float:
                    {
                        if (!isNull)
                        {
                            double v1 = it1.DoubleValue();
                            double v2 = it2.DoubleValue();
                            if (!double.IsNaN(v1) && !double.IsNaN(v2))
                            {
                                Assert.AreEqual(v1, v2, 0);

                                // The last param is a delta, and we want exact match.
                            }
                        }

                        break;
                    }

                case IonType.Decimal:
                    {
                        if (!isNull)
                        {
                            BigDecimal bigDec1 = it1.DecimalValue();
                            BigDecimal bigDec2 = it2.DecimalValue();
                            AssertPreciselyEquals(bigDec1, bigDec2);

                            decimal dec1 = bigDec1.ToDecimal();
                            decimal dec2 = bigDec2.ToDecimal();
                            Assert.AreEqual(dec1, dec2);

                            Assert.AreEqual(decimal.ToDouble(dec1), decimal.ToDouble(dec2));
                            Assert.AreEqual(decimal.ToInt32(dec1), decimal.ToInt32(dec2));
                            Assert.AreEqual(decimal.ToInt64(dec1), decimal.ToInt64(dec2));
                        }

                        break;
                    }

                case IonType.Timestamp:
                    {
                        if (!isNull)
                        {
                            Timestamp t1 = it1.TimestampValue();
                            Timestamp t2 = it2.TimestampValue();
                            Assert.AreEqual(t1, t2);
                        }

                        break;
                    }

                case IonType.String:
                    {
                        string s1 = it1.StringValue();
                        string s2 = it2.StringValue();
                        Assert.AreEqual(s1, s2);
                        break;
                    }

                case IonType.Symbol:
                    {
                        SymbolToken tok1 = it1.SymbolValue();
                        SymbolToken tok2 = it2.SymbolValue();
                        if (isNull)
                        {
                            Assert.IsNull(tok1.Text);
                            Assert.IsNull(tok2.Text);
                        }
                        else if (tok1.Text == null || tok2.Text == null)
                        {
                            Assert.AreEqual(tok1.Sid, tok2.Sid, "sids");
                        }
                        else
                        {
                            string s1 = tok1.Text;
                            string s2 = tok2.Text;
                            Assert.AreEqual(s1, s2);
                        }

                        break;
                    }

                case IonType.Blob:
                case IonType.Clob:
                    {
                        if (!isNull)
                        {
                            byte[] b1 = it1.NewByteArray();
                            byte[] b2 = it2.NewByteArray();
                            Assert.IsTrue(b1 != null && b2 != null);
                            Assert.IsTrue(b1.Length == b2.Length);
                            for (int ii = 0; ii < b1.Length; ii++)
                            {
                                byte v1 = b1[ii];
                                byte v2 = b2[ii];
                                Assert.AreEqual(v1, v2);
                            }
                        }

                        break;
                    }

                default:
                    throw new InvalidOperationException("iterated to a type that's not expected");
            }
        }

        /******** from IonAssert ********/

        private static void AssertSymbolEquals(string path, SymbolToken[] expecteds, SymbolToken[] actuals)
        {
            Assert.AreEqual(expecteds.Length, actuals.Length, path + "count");

            for (int i = 0; i < expecteds.Length; i++)
            {
                AssertSymbolEquals(path + "[" + i + "]", expecteds[i], actuals[i]);
            }
        }

        private static void AssertSymbolEquals(string path, SymbolToken expected, SymbolToken actual)
        {
            string expectedText = expected.Text;
            string actualText = actual.Text;
            Assert.AreEqual(expectedText, actualText, path + " text");

            if (expectedText == null)
            {
                Assert.AreEqual(expected.Sid, actual.Sid, path + " sid");
            }
        }

        private static void AssertPreciselyEquals(BigDecimal expected, BigDecimal actual)
        {
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.IsNegativeZero, actual.IsNegativeZero, "isNegativeZero");
            Assert.IsTrue(expected.Equals(actual));

            // TODO scaled and unscaled values, currently IonDotNet has internal access modifier for those fields.
        }
    }
}
