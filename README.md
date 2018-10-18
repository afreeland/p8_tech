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

