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

        string CurrentFieldName
        {
            get;
        }

        bool CurrentIsNull
        {
            get;
        }

        IonType CurrentType
        {
            get;
        }

        dynamic CurrentValue
        {
            get;
        }
    }
}
