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
1. Replace the console application with a GUI variant (consider using .NET Maui)
2. Add searchable dropdown lists to select the from/to branches or tags.
3. Dropdown lists should be a distinct list of all branches and tags taken from all of the repos.
4. Options that are not common to all repos should be highlighted in some manner (ability to select different from/to branches per repo?)
5. Notify users of repository-level progress (can split the GUI in two, having repos & branches on the left and then a console window on the right where the in-progress detail can be added)
6. Allow overriding the from/to branch at the repo level (for when there are inconsistencies across repos)
7. Update the file saving logic to output to a .csv (check with the team for what would be desired here)
8. Rewrite the diff generation logic to execute asynchronously
9. Rewrite some of the diff reference extraction logic to be more dynamic/configurable (project agnostic)
10. Update & complete the remaining unit tests
11. Update documentation
