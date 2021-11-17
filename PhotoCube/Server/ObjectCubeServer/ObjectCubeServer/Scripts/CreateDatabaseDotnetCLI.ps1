# dotnet tool install --global dotnet-ef
dotnet ef database drop
dotnet ef migrations remove

dotnet ef migrations add init

dotnet ef database update

