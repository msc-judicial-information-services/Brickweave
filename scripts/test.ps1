$status = 0
$rootDir = (get-item $PSScriptRoot).Parent.FullName
$testProjects = Get-ChildItem -Path $rootDir\tests -Filter *.csproj -Recurse -File| ForEach-Object { $_.FullName }

foreach ($testProject in $testProjects)
{
    dotnet test $testProject --no-build

    if ($status -eq 0) {
        $status = $LASTEXITCODE
    }
}

exit $status