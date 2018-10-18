# p8_tech
Quick app

## Setup SQL Server

Pull latest SQL images
```
docker pull microsoft/mssql-server-linux
```

Lets run our container by:
* Accepting EULA
* Setting a SA password
* Exposing port

```
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=1superSecret' -p 1433:1433 -d microsoft/mssql-server-linux:latest
```

## Setup solution

Create a new solution
```
dotnet new sln
```

Create a class library
```
dotnet new classlib -o library
```

Add our new class library to our solution
```
dotnet sln add library/library.csproj
```


We need the following 4 dependencies
```
dotnet add library package System.Data.SqlClient
dotnet add library package Microsoft.Extensions.Configuration.FileExtensions
dotnet add library package Microsoft.Extensions.Configuration.Json
dotnet add library package Newtonsoft.Json
```


Need to use the restore command for 'unresolved dependencies'
```
dotnet restore
```

Create our console app
```
dotnet new console -o app
```

Add our new console app to our main solution
```
dotnet sln add app/app.csproj
```

Reference our class library in our console app
```
dotnet add app/app.csproj reference library/library.csproj
```

Run application
```
dotnet run -p app/app.csproj
```