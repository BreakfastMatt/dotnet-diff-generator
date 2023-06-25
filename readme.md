# Git Diff Generation Tool

This is a .NET 6 console application that generates Git diffs between user-specified branches or tags of multiple project repositories. It relies on a JSON configuration file to specify the locations of each project repository.

## How to run

TODO - coming soon :)

## Configuration

The application relies on the **config.json** file to specify the locations of the project repositories. Update this file with the appropriate repository paths and any other required configuration settings.

Example **`config.json`**:

```json
{
  "Repositories": [
    {
      "Name": "Web",
      "Path": "C:/Path/To/Repository1"
    },
    {
      "Name": "Integration",
      "Path": "C:/Path/To/Repository2"
    }
  ],
  "OutputPath": "C:/Path/To/Output"
}
```

## Build the Application

In visual studio, open the terminal and run this command:

```powershell
dotnet publish -c Release -r win-x64
```

if the above command succeeded then open a terminal in the Git-Diff-Generator\Console folder and run this command:

```powershell
dotnet publish ConsoleApp.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true --output "C:\Users\MLewis\Desktop\files"
```
