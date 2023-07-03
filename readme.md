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

To build this application you either need to run the **`publish-script.ps1`** file or you need to navigate to the Git-Diff-Generator\Console folder and run the following command:

```powershell
dotnet publish ConsoleApp.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true --output "C:\Users\MLewis\Desktop\files"
```

## Upcoming Features
1. Branch & Tag name prompting when entering from/to references (e.g. double tap tab to show branches/tags with that naming)
2. Improve the UX (notify users of repository-level progress)
3. Add GUI
4. Rewrite some of the logic to utilise things like Linq queries instead of nested loops
5. Fix the console output timing inconsistencies (requiring user input for the buffer to be flushed to the console)
6. Update & complete remaining unit tests
7. Add logic to make the diff reference extraction and grouping logic dynamic/configurable (project agnostic)
8. Update logic to output a .csv file instead of a text file
