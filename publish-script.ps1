# Change directory to the Git-Diff-Generator folder
Set-Location -Path '.\'

# Run dotnet publish for the Git-Diff-Generator project
dotnet publish -c Release -r win-x64

# Change directory to the Console folder
Set-Location -Path '.\Console'

# Run dotnet publish for the ConsoleApp.csproj project
dotnet publish ConsoleApp.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true --output 'C:\GitDiffs\PublishedFiles'
