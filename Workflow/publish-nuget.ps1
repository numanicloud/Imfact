$project = "..\Dev\Imfact\Imfact.csproj"

$version = nbgv get-version -p $project -v NuGetPackageVersion
dotnet pack $project -o Pack/ -v minimal -p:PackageVersion=$version

$key = Get-Content ./NuGetApiKey.txt

dotnet nuget push "Pack/Imfact.${version}.nupkg" -k $key -s https://api.nuget.org/v3/index.json