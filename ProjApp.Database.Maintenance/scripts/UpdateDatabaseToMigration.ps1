$migrationName = read-host "Migration name"
if ([string]::IsNullOrWhiteSpace($migrationName)) {
	write-error "Wrong migration name" -ErrorAction Stop
}
dotnet ef database update $migrationName