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

## Upcoming features

1. Prompt user for a from and to branch/tag
2. Utilise the config file details to fetch the latest for the *'from'* & *'to'* branches
3. Add error checking for project repos that don't have the branch or tag
4. Generate diffs for each project
5. Add error checking for the diff generation
6. Combine all the diffs into a single file
7. Format the diffs (Centralise this logic somewhere so it is easy to alter)
8. Group the commits (Centralise the grouping logic so it is easy to alter)
9. Enhance the config file to specify a default output folder
