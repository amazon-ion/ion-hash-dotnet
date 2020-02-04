namespace IonHashDotnet.Tests
{
    using System.Linq;
    using IonDotnet;
    using IonDotnet.Tree;
    using IonDotnet.Tree.Impl;

    public abstract class TestIonHasherProvider : IIonHasherProvider
    {
        private static readonly ValueFactory valueFactory = new ValueFactory();
        private protected readonly IIonValue hashLog = valueFactory.NewEmptySexp();

        public static TestIonHasherProvider GetInstance(string algorithm)
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

        internal static void AddHashToLog(IIonValue hashLog, string method, byte[] hash)
        {
            var node = valueFactory.NewEmptySexp();
            node.AddTypeAnnotation(new SymbolToken(method, SymbolToken.UnknownSid));
            foreach (byte b in hash)
            {
                node.Add(valueFactory.NewInt(b & 0xFF));
            }

            hashLog.Add(node);
        }

        public abstract IIonHasher NewHasher();

        public IIonValue GetHashLog()
        {
            return this.hashLog;
        }
    }

    internal class IdentityIonHasherProvider : TestIonHasherProvider
    {
        public override IIonHasher NewHasher()
        {
            return new IdentityIonHasher(this.hashLog);
        }

        private class IdentityIonHasher : IIonHasher
        {
            IIonValue hashLog;
            byte[] identityHash;

            public IdentityIonHasher(IIonValue hashLog)
            {
                this.hashLog = hashLog;
                identityHash = new byte[0];
            }

            public void Update(byte[] bytes)
            {
                AddHashToLog(hashLog, "update", bytes);
                this.identityHash = this.identityHash.Concat(bytes).ToArray();
            }

            public byte[] Digest()
            {
                byte[] bytes = this.identityHash;
                this.identityHash = new byte[0];
                AddHashToLog(this.hashLog, "digest", bytes);
                return bytes;
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
            return new DefaultTestIonHasher(this.algorithm, this.hashLog);
        }

        private class DefaultTestIonHasher : CryptoIonHasher
        {
            IIonValue hashLog;

            public DefaultTestIonHasher(string algorithm, IIonValue hashLog) : base(algorithm)
            {
                this.hashLog = hashLog;
            }

            public override void Update(byte[] bytes)
            {
                AddHashToLog(this.hashLog, "update", bytes);
                base.Update(bytes);
            }

            public override byte[] Digest()
            {
                byte[] hash = base.Digest();
                AddHashToLog(this.hashLog, "digest", hash);
                return hash;
            }
        }
    }
}
