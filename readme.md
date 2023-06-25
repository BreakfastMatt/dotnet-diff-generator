# Git Diff Generation Tool

This .NET 6 console application generates Git diffs between user-specified branches or tags of multiple project repositories. It relies on a JSON configuration file to specify the locations of each project repository. You must have **`git`** installed on your system and set up your repository authentication to use ssh.

## How to run

First update the **`config.json`** file to add all the information for your repositories.

Execute the **`GenerateDiffs.exe`** file and enter the relevant information for the diff you are trying to generate. 

## Configuration

The application relies on the **config.json** file to specify the locations of the project repositories. Update this file with the appropriate repository paths and configuration settings.

Example **`config.json`**:

```json
{
  "RepositoryDetails": [
    {
      "Name": "Web",
      "MainBranchName": "dev",
      "Path":  "C:\\Path\\To\\Repository2"
    },
    {
      "Name": "IntegrationApi",
      "MainBranchName": "main",
      "Path": "C:\\Path\\To\\Repository2"
    }
  ],
  "OutputPath": "C:\\Path\\To\\Output"
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

## Upcoming Features
1. Improve the UX (notify users of repository level progress)
2. Rewrite some of the logic to utilise things like Linq queries instead of nested loops
3. Fix the console output timing inconsistencies (requiring user input for the buffer to be flushed to the console)
