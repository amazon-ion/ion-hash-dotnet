using System;
using IonDotnet;
using IonDotnet.Tree;
using IonDotnet.Tree.Impl;

namespace IonHashDotnet.Tests
{
    internal class TestIonHasherProviders
    {
        private static TestIonHasherProvider GetInstance(string algorithm)
        {
            if (algorithm.Equals("identity"))
            {
                return new IdentityIonHasherProvider();
            }
            else
            {
                return new DefaultTestIonHasherProvider(algorithm);
            }
        }

        internal abstract class TestIonHasherProvider : IIonHasherProvider
        {
            private static readonly ValueFactory valueFactory = new ValueFactory();
            private readonly IIonValue hashLog = valueFactory.NewEmptySexp();

            public abstract IIonHasher NewHasher();

            public void AddHashToLog(string method, byte[] hash)
            {
                var node = valueFactory.NewEmptySexp();
                node.AddTypeAnnotation(new SymbolToken(method, SymbolToken.UnknownSid));
                foreach (byte b in hash)
                {
                    node.Add(valueFactory.NewInt(b & 0xFF));
                }

                hashLog.Add(node);
            }

            public IIonValue GetHashLog()
            {
                return this.hashLog;
            }
        }

        internal class IdentityIonHasherProvider : TestIonHasherProvider
        {
            public override IIonHasher NewHasher()
            {
                return new IdentityIonHasher();
            }

            private class IdentityIonHasher : IIonHasher
            {
                public byte[] Hash => throw new NotImplementedException();

                public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
                {
                    throw new NotImplementedException();
                }

                public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
                {
                    throw new NotImplementedException();
                }
            }
        }

        internal class DefaultTestIonHasherProvider : TestIonHasherProvider
        {
            private string algorithm;

            public DefaultTestIonHasherProvider(string algorithm)
            {
                this.algorithm = algorithm;
            }

            public override IIonHasher NewHasher()
            {
                return new DefaultTestIonHasher(algorithm);
            }

            private class DefaultTestIonHasher : IIonHasher
            {
                public DefaultTestIonHasher(string algorithm)
                {

                }

                public byte[] Hash => throw new NotImplementedException();

                public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
                {
                    throw new NotImplementedException();
                }

                public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}
