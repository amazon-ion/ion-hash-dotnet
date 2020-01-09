namespace IonHashDotnet
{
    using System.Collections.Generic;
    using IonDotnet;

    internal interface IIonHashValue
    {
        IList<SymbolToken> Annotations
        {
            get;
        }

        string FieldName
        {
            get;
        }

        bool IsNull
        {
            get;
        }

        IonType Type
        {
            get;
        }

        dynamic Value
        {
            get;
        }
    }
}
