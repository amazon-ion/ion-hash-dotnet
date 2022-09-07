# Amazon Ion Hash .NET

An implementation of [Amazon Ion Hash](http://amzn.github.io/ion-hash) in C#.

[![Build Status](https://github.com/amazon-ion/ion-hash-dotnet/workflows/Ion%20Hash%20DotNet%20CI/badge.svg)](https://github.com/amazon-ion/ion-hash-dotnet/actions?query=workflow%3A%22Ion+Hash+DotNet+CI%22)
[![nuget version](https://img.shields.io/nuget/v/Amazon.IonHashDotnet)](https://www.nuget.org/packages/Amazon.IonHashDotnet)
[![license](https://img.shields.io/hexpm/l/plug.svg)](https://github.com/amazon-ion/ion-hash-dotnet/blob/master/LICENSE)

## Getting Started

This library targets [.NET Standard 2.0](https://dotnet.microsoft.com/platform/dotnet-standard).  The following example code
illustrates how to use it:

```C#
class HashDuringWriteAndRead
{
    static void Main(string[] args)
    {
        IIonHasherProvider hasherProvider = new CryptoIonHasherProvider("SHA-256");

        // write a simple Ion struct and compute the hash
        using TextWriter tw = new StringWriter();
        using IIonWriter writer = IonTextWriterBuilder.Build(tw);
        using IIonHashWriter hashWriter = IonHashWriterBuilder.Standard()
            .WithHasherProvider(hasherProvider)
            .WithWriter(writer)
            .Build();

        Console.WriteLine("writer");
        hashWriter.StepIn(IonType.Struct);
        hashWriter.SetFieldName("first_name");
        hashWriter.WriteString("Amanda");
        hashWriter.SetFieldName("middle_name");
        hashWriter.WriteString("Amanda");
        hashWriter.SetFieldName("last_name");
        hashWriter.WriteString("Smith");
        hashWriter.StepOut();
        Console.WriteLine(BytesToHex(hashWriter.Digest()));

        string ionString = tw.ToString();
        Console.WriteLine("Ion data: " + ionString);


        // read the struct and compute the hash
        using IIonReader reader = IonReaderBuilder.Build(ionString);
        using IIonHashReader hashReader = IonHashReaderBuilder.Standard()
            .WithReader(reader)
            .WithHasherProvider(hasherProvider)
            .Build();

        Console.WriteLine("reader");
        hashReader.MoveNext();    // position reader at the first value
        hashReader.MoveNext();    // position reader just after the struct
        Console.WriteLine(BytesToHex(hashReader.Digest()));
    }

    private static string BytesToHex(byte[] bytes)
    {
        return BitConverter.ToString(bytes).Replace("-", " ").ToLower();
    }
}
```

Upon execution, the above code produces the following output:
```
writer
37 82 6e 71 92 a1 e4 e1 24 aa 73 f9 85 0f f1 0f 1c b5 cc ca f2 07 b0 9e 65 af 42 56 ae 8c 80 55
Ion data: {first_name:"Amanda",middle_name:"Amanda",last_name:"Smith"}
reader
37 82 6e 71 92 a1 e4 e1 24 aa 73 f9 85 0f f1 0f 1c b5 cc ca f2 07 b0 9e 65 af 42 56 ae 8c 80 55
```

## Development

This repository contains a [git submodule](https://git-scm.com/docs/git-submodule)
called `ion-hash-test`, which holds test data used by `ion-hash-dotnet`'s unit tests.

The easiest way to clone the `ion-hash-dotnet` repository and initialize its `ion-hash-test`
submodule is to run the following command:

```
$ git clone --recursive https://github.com/amazon-ion/ion-hash-dotnet.git ion-hash-dotnet
```

Alternatively, the submodule may be initialized independently from the clone
by running the following commands:

```
$ git submodule init
$ git submodule update
```


## Known Issues

Any tests commented out in [IonHashDotnet.Tests/ion_hash_tests.ion](https://github.com/amazon-ion/ion-hash-dotnet/blob/master/IonHashDotnet.Tests/ion_hash_tests.ion)
are not expected to work at this time.


## License

This library is licensed under the Apache-2.0 License.
