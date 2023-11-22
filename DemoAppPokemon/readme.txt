
add-migration
update-database

dotnet ef migrations add 'vendor-register'
dotnet ef database update


dotnet ef migrations add Initial