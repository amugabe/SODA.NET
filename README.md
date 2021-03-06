# SODA.NET [![Build status](https://ci.appveyor.com/api/projects/status/yub6lyl79573lufv/branch/master?svg=true)](https://ci.appveyor.com/project/thekaveman/soda-net/branch/master)

A [Socrata Open Data API](https://dev.socrata.com) (SODA) client library targeting .NET 4.5 and above.

## Getting Started

SODA.NET is available as a [NuGet package](https://www.nuget.org/packages/CSM.SodaDotNet/).

```console
dotnet add package CSM.SodaDotNet
```

## Usage examples

Simple, read-only access

```c#
//initialize a new client
//make sure you register for your own app token (http://dev.socrata.com/register)
var client = new SodaClient("data.smgov.net", "REPLACE_WITH_YOUR_APP_TOKEN");

//read metadata of a dataset using the resource identifier (Socrata 4x4)
var metadata = client.GetMetadata("1234-wxyz");
Console.WriteLine("{0} has {1} views.", metadata.Name, metadata.ViewsCount);

//get a reference to the resource itself
//the result (a Resouce object) is a generic type
//the type parameter represents the underlying rows of the resource
var dataset = client.GetResource<Dictionary<string, object>>("1234-wxyz");

//of course, a custom type can be used as long as it is JSON serializable
var dataset = client.GetResource<MyClass>("1234-wxyz");

//Resource objects read their own data
var allRows = dataset.GetRows();
var first10Rows = dataset.GetRows(10);

//collections of an arbitrary type can be returned
//using SoQL and a fluent query building syntax
var soql = new SoqlQuery().Select("column1", "column2")
                          .Where("something > nothing")
                          .Group("column3");

var results = dataset.Query<MyOtherClass>(soql);
```

**SodaClient** is also capable of performing write operations

```c#
//make sure to provide auth credentials!
var client = 
    new SodaClient("data.smgov.net", "AppToken", "user@domain.com", "password");

//Upsert some data serialized as CSV
string csvData = File.ReadAllText("data.csv");
client.Upsert(csvData, SodaDataFormat.CSV, "1234-wxyz");

//Upsert a collection of serializable entities
IEnumerable<MyClass> payload = GetPayloadData();
client.Upsert(payload, "1234-wxyz");
```

Note: This library supports writing directly to datasets with the Socrata Open Data API. For write operations that use
data transformations in the Socrata Data Management Experience (the user interface for creating datasets), use the Socrata
Data Management API. For more details on when to use SODA vs the Socrata Data Management API, see the
[Data Management API documentation](https://socratapublishing.docs.apiary.io/#)

## Build

Compilation can be done using
[Visual Studio Community Edition](https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx).

You can also use the .NET CLI:

```console
git clone git@github.com:CityofSantaMonica/SODA.NET.git SODA.NET
cd SODA.NET
dotnet build
```

## Tests

[NUnit](http://nunit.org/) was used to build and run the test projects.

Run tests from the Visual Studio Test Explorer, or using the .NET CLI:

```console
dotnet test
```

## Contributing

Check out the [Contributor Guidelines](https://github.com/CityOfSantaMonica/SODA.NET/blob/master/CONTRIBUTING.md)
for more details.

## Copyright and License

Copyright 2019 City of Santa Monica, CA

Licensed under the [MIT License](https://github.com/CityOfSantaMonica/SODA.NET/blob/master/LICENSE.txt)

## Thank you

A tremendous amount of inspiration for this project came from the following projects. Thank you!

- [Octokit.net](https://github.com/octokit/octokit.net)
- [soda-java](https://github.com/socrata/soda-java/)
- [soda-dotnet](https://github.com/socrata/soda-dotnet) (defunct?)
