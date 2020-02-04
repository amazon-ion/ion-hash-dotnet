namespace IonHashDotnet.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using IonDotnet;
    using IonDotnet.Builders;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BigListOfNaughtyStringsTest
    {
        private static readonly IIonHasherProvider HasherProvider = new CryptoIonHasherProvider("SHA-256");

        [DataTestMethod]
        [BigNaughtyListOfStringsDataSource]
        public void Test(TestValue tv, string s)
        {
            IIonHashWriter hashWriter = null;
            try
            {
                hashWriter = IonHashWriterBuilder.Standard()
                    .WithWriter(IonTextWriterBuilder.Build(new StringWriter()))
                    .WithHasherProvider(HasherProvider)
                    .Build();
                hashWriter.WriteValues(IonReaderBuilder.Build(s));
            }
            catch (IonException e)
            {
                if (tv.IsValidIon())
                {
                    throw e;
                }
            }

            IIonHashReader hashReader = null;
            try
            {
                hashReader = IonHashReaderBuilder.Standard()
                    .WithReader(IonReaderBuilder.Build(s))
                    .WithHasherProvider(HasherProvider)
                    .Build();
                hashReader.MoveNext();
                hashReader.MoveNext();
            }
            catch (IonException e)
            {
                if (tv.IsValidIon())
                {
                    throw e;
                }
            }

            if (tv.validIon == null || tv.validIon.Value == true)
            {
                TestUtil.AssertEquals(
                    hashWriter.Digest(),
                    hashReader.Digest(),
                    "Reader/writer hashes for line |" + tv.AsIon() + "| as |" + s + "| don't match");
            }
        }


        public class BigNaughtyListOfStringsDataSource : Attribute, ITestDataSource
        {
            public IEnumerable<object[]> GetData(MethodInfo methodInfo)
            {
                List<object[]> list = new List<object[]>();

                try
                {
                    var file = DirStructure.IonHashTestFile("big_list_of_naughty_strings.txt");
                    var fileStream = file.OpenRead();
                    using (StreamReader sr = new StreamReader(fileStream))
                    {
                        while (!sr.EndOfStream)
                        {
                            String line = sr.ReadLine();
                            if (line.StartsWith("#") || string.IsNullOrEmpty(line))
                            {
                                continue;
                            }
                            TestValue tv = new TestValue(line);

                            list.Add(new object[] { tv, tv.AsSymbol() });
                            list.Add(new object[] { tv, tv.AsString() });
                            list.Add(new object[] { tv, tv.AsLongString() });
                            list.Add(new object[] { tv, tv.AsClob() });
                            list.Add(new object[] { tv, tv.AsBlob() });

                            list.Add(new object[] { tv, tv.AsSymbol() + "::" + tv.AsSymbol() });
                            list.Add(new object[] { tv, tv.AsSymbol() + "::" + tv.AsString() });
                            list.Add(new object[] { tv, tv.AsSymbol() + "::" + tv.AsLongString() });
                            list.Add(new object[] { tv, tv.AsSymbol() + "::" + tv.AsClob() });
                            list.Add(new object[] { tv, tv.AsSymbol() + "::" + tv.AsBlob() });

                            list.Add(new object[] { tv, tv.AsSymbol() + "::{" + tv.AsSymbol() + ":" + tv.AsSymbol() + "}" });
                            list.Add(new object[] { tv, tv.AsSymbol() + "::{" + tv.AsSymbol() + ":" + tv.AsString() + "}" });
                            list.Add(new object[] { tv, tv.AsSymbol() + "::{" + tv.AsSymbol() + ":" + tv.AsLongString() + "}" });
                            list.Add(new object[] { tv, tv.AsSymbol() + "::{" + tv.AsSymbol() + ":" + tv.AsClob() + "}" });
                            list.Add(new object[] { tv, tv.AsSymbol() + "::{" + tv.AsSymbol() + ":" + tv.AsBlob() + "}" });

                            if (tv.IsValidIon())
                            {
                                list.Add(new object[] { tv, tv.AsIon() });
                                list.Add(new object[] { tv, tv.AsSymbol() + "::" + tv.AsIon() });
                                list.Add(new object[] { tv, tv.AsSymbol() + "::{" + tv.AsSymbol() + ":" + tv.AsIon() + "}" });
                                list.Add(new object[] { tv, tv.AsSymbol() + "::{" + tv.AsSymbol() + ":" + tv.AsSymbol() + "::" + tv.AsIon() + "}" });
                            }

                            // list
                            list.Add(new object[]
                            {
                                tv,
                                tv.AsSymbol() + "::["
                                + tv.AsSymbol() + ", "
                                + tv.AsString() + ", "
                                + tv.AsLongString() + ", "
                                + tv.AsClob() + ", "
                                + tv.AsBlob() + ", "
                                + (tv.IsValidIon() ? tv.AsIon() : "")
                                + "]"
                            });

                            // sexp
                            list.Add(new object[]
                            {
                                tv,
                                tv.AsSymbol() + "::("
                                + tv.AsSymbol() + " "
                                + tv.AsString() + " "
                                + tv.AsLongString() + " "
                                + tv.AsClob() + " "
                                + tv.AsBlob() + " "
                                + (tv.IsValidIon() ? tv.AsIon() : "")
                                + ")"
                            });

                            // multiple annotations
                            list.Add(new object[] { tv, tv.AsSymbol() + "::" + tv.AsSymbol() + "::" + tv.AsSymbol() + "::" + tv.AsString() });

                        }
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                }
                return list;
            }

            public string GetDisplayName(MethodInfo methodInfo, object[] data)
            {
                return string.Format(CultureInfo.CurrentCulture, "Custom - {0} ({1})", methodInfo.Name, string.Join(",", data[1]));
            }
        }


        public class TestValue
        {
            private const string IonPrefix = "ion::";
            private const string InvalidIonPrefix = "invalid_ion::";

            private readonly string ion;
            internal bool? validIon;

            internal TestValue(string ion)
            {
                this.ion = ion;

                if (this.ion.StartsWith(IonPrefix))
                {
                    validIon = true;
                    this.ion = this.ion.Substring(IonPrefix.Length);
                }
                if (this.ion.StartsWith(InvalidIonPrefix))
                {
                    validIon = false;
                    this.ion = this.ion.Substring(InvalidIonPrefix.Length);
                }
            }

            internal string AsIon()
            {
                return ion;
            }

            internal string AsSymbol()
            {
                string s = ion;
                s = s.Replace("\\", "\\\\");
                s = s.Replace("'", "\\'");
                s = "\'" + s + "\'";
                return s;
            }

            internal string AsString()
            {
                string s = ion;
                s = s.Replace("\\", "\\\\");
                s = s.Replace("\"", "\\\"");
                s = "\"" + s + "\"";
                return s;
            }

            internal string AsLongString()
            {
                string s = ion;
                s = s.Replace("\\", "\\\\");
                s = s.Replace("'", "\\'");
                return "'''" + s + "'''";
            }

            internal string AsClob()
            {
                string s = AsString();
                StringBuilder sb = new StringBuilder();
                foreach (byte b in Encoding.Default.GetBytes(s))
                {
                    int c = b & 0xFF;
                    if (c >= 128)
                    {
                        sb.Append("\\x").Append(c.ToString());
                    }
                    else
                    {
                        sb.Append((char)c);
                    }
                }
                return "{{" + sb.ToString() + "}}";
            }

            internal string AsBlob()
            {
                return "{{" + Convert.ToBase64String(Encoding.Default.GetBytes(ion)) + "}}";
            }

            internal bool IsValidIon()
            {
                return validIon != null && validIon.Value;
            }
        }

    }
}
