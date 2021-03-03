$project = "..\Dev\Imfact\Imfact.csproj"

$version = nbgv get-version -p $project -v NuGetPackageVersion
dotnet pack $project -o Pack/ -v minimal -p:PackageVersion=$version

dotnet nuget push "Pack/Imfact.${version}.nupkg" -s D:\Home\MyDocuments\LocalNugetFeed\Imfact