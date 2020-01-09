namespace IonHashDotnet.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using IonDotnet;
    using IonDotnet.Systems;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BigListOfNaughtyStringsTest
    {
        private static readonly IIonHasherProvider HasherProvider = new CryptoIonHasherProvider("SHA-256");

        [DataTestMethod]
        [BigNaughtyListOfStringsDataSource]
        public void Test(TestValue tv, string s)
        {
            IIonHashWriter HashWriter = null;
            try
            {
                HashWriter = IonHashWriterBuilder.Standard()
                    .WithWriter(IonTextWriterBuilder.Build(new StringWriter()))
                    .WithHasherProvider(HasherProvider)
                    .Build();
                HashWriter.WriteValues(IonReaderBuilder.Build(s));
            }
            catch (IonException e)
            {
                if (tv.IsValidIon())
                {
                    throw e;
                }
            }

            IIonHashReader HashReader = null;
            try
            {
                HashReader = IonHashReaderBuilder.Standard()
                    .WithReader(IonReaderBuilder.Build(s))
                    .WithHasherProvider(HasherProvider)
                    .Build();
                HashReader.MoveNext();
                HashReader.MoveNext();
            }
            catch (IonException e)
            {
                if (tv.IsValidIon())
                {
                    throw e;
                }
            }

            if (tv.ValidIon == null || tv.ValidIon.Value == true)
            {
                TestUtil.AssertEquals(
                    HashWriter.Digest(),
                    HashReader.Digest(),
                    "Reader/writer hashes for line |" + tv.AsIon() + "| as |" + s + "| don't match");
            }
        }

        
        public class BigNaughtyListOfStringsDataSource : Attribute, ITestDataSource
        {
            public IEnumerable<object[]> GetData(MethodInfo methodInfo)
            {
                List<object[]> List = new List<object[]>();

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

                            List.Add(new Object[] { tv, tv.AsSymbol() });
                            List.Add(new object[] { tv, tv.AsString() });
                            List.Add(new object[] { tv, tv.AsLongString() });
                            List.Add(new object[] { tv, tv.AsClob() });
                            List.Add(new object[] { tv, tv.AsBlob() });

                            List.Add(new object[] { tv, tv.AsSymbol() + "::" + tv.AsSymbol() });
                            List.Add(new object[] { tv, tv.AsSymbol() + "::" + tv.AsString() });
                            List.Add(new object[] { tv, tv.AsSymbol() + "::" + tv.AsLongString() });
                            List.Add(new object[] { tv, tv.AsSymbol() + "::" + tv.AsClob() });
                            List.Add(new object[] { tv, tv.AsSymbol() + "::" + tv.AsBlob() });

                            List.Add(new object[] { tv, tv.AsSymbol() + "::{" + tv.AsSymbol() + ":" + tv.AsSymbol() + "}" });
                            List.Add(new object[] { tv, tv.AsSymbol() + "::{" + tv.AsSymbol() + ":" + tv.AsString() + "}" });
                            List.Add(new object[] { tv, tv.AsSymbol() + "::{" + tv.AsSymbol() + ":" + tv.AsLongString() + "}" });
                            List.Add(new object[] { tv, tv.AsSymbol() + "::{" + tv.AsSymbol() + ":" + tv.AsClob() + "}" });
                            List.Add(new object[] { tv, tv.AsSymbol() + "::{" + tv.AsSymbol() + ":" + tv.AsBlob() + "}" });

                            if (tv.IsValidIon())
                            {
                                List.Add(new object[] { tv, tv.AsIon() });
                                List.Add(new object[] { tv, tv.AsSymbol() + "::" + tv.AsIon() });
                                List.Add(new object[] { tv, tv.AsSymbol() + "::{" + tv.AsSymbol() + ":" + tv.AsIon() + "}" });
                                List.Add(new object[] { tv, tv.AsSymbol() + "::{" + tv.AsSymbol() + ":" + tv.AsSymbol() + "::" + tv.AsIon() + "}" });
                            }

                            // list
                            List.Add(new object[]
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
                            List.Add(new object[]
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
                            List.Add(new object[] { tv, tv.AsSymbol() + "::" + tv.AsSymbol() + "::" + tv.AsSymbol() + "::" + tv.AsString() });

                        }
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                }
                return List;
            }


            public string GetDisplayName(MethodInfo methodInfo, object[] data)
            {
                if (data != null)
                return string.Format(CultureInfo.CurrentCulture, "Custom - {0} ({1})", methodInfo.Name, string.Join(",", data[1]));

                return null;
            }
        }


        public class TestValue
        {
            private const string IonPrefix = "ion::";
            private const string InvalidIonPrefix = "invalid_ion::";

            private string Ion;
            internal bool? ValidIon;

            internal TestValue(string ion)
            {
                this.Ion = ion;

                if (this.Ion.StartsWith(IonPrefix))
                {
                    ValidIon = true;
                    this.Ion = this.Ion.Substring(IonPrefix.Length);
                }
                if (this.Ion.StartsWith(InvalidIonPrefix))
                {
                    ValidIon = false;
                    this.Ion = this.Ion.Substring(InvalidIonPrefix.Length);
                }
            }

            internal string AsIon()
            {
                return Ion;
            }

            internal string AsSymbol()
            {
                string s = Ion;
                s = s.Replace("\\", "\\\\");
                s = s.Replace("'", "\\'");
                s = "\'" + s + "\'";
                return s;
            }

            internal string AsString()
            {
                string s = Ion;
                s = s.Replace("\\", "\\\\");
                s = s.Replace("\"", "\\\"");
                s = "\"" + s + "\"";
                return s;
            }

            internal string AsLongString()
            {
                string s = Ion;
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
                return "{{" + Convert.ToBase64String(Encoding.Default.GetBytes(Ion)) + "}}";
            }

            internal bool IsValidIon()
            {
                return ValidIon != null && ValidIon.Value;
            }
        }

    }
}
