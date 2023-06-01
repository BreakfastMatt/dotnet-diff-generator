# Git Diff Generation Tool

This is a .NET 6 console application that generates Git diffs between user-specified branches or tags of multiple project repositories. It relies on a JSON configuration file to specify the locations of each project repository.

## Configuration

The application relies on the **config.json** file to specify the locations of the project repositories. Update this file with the appropriate repository paths and any other required configuration settings.

Example **`config.json`**:

```json
{
  "repositories": [
    {
      "name": "Project A",
      "path": "/path/to/project-a"
    },
    {
      "name": "Project B",
      "path": "/path/to/project-b"
    }
  ]
}