#!/bin/sh
dotnet ef database drop --project ObjectCubeServer.csproj
rm -rf Migrations/
dotnet ef migrations add init --project ObjectCubeServer.csproj --startup-project ObjectCubeServer.csproj
dotnet ef database update --project ObjectCubeServer.csproj --startup-project ObjectCubeServer.csproj
