namespace Amazon.IonHash
{
    using System.Collections.Generic;
    using IonDotnet;

    // TODO delete this interface once IonDotnet has IIonValue updated to interface
    public interface IIonValue
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