param(
    [Parameter(Mandatory = $true)]
    [string]$MigrationName
)

dotnet tool update --global dotnet-ef

dotnet ef migrations add $MigrationName `
    --project ./HomeStack.Database.SqlServer/HomeStack.Database.SqlServer.csproj `
    --startup-project ./HomeStack.Api/HomeStack.Api.csproj `
    --context SqlServerDbContext `
    --output-dir Migrations
	
dotnet ef migrations add $MigrationName `
    --project ./HomeStack.Database.Postgres/HomeStack.Database.Postgres.csproj `
    --startup-project ./HomeStack.Api/HomeStack.Api.csproj `
    --context PostgresDbContext `
    --output-dir Migrations
