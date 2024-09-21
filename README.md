# BuildSphere

BuildSphere is a simple dotnet tool for building and deploying .NET applications to NuGet and GitHub.

## Dependencies

- Nuke
- MinVer
- Git-Cliff

You need to set up a GitHub secret with the NuGet API key. The secret should be named `NUGET_API_KEY`.

## Installation

- Clone this repository
- From the `./build` directory, run `dotnet pack -c Release`
- The tool is now available as a NuGet package in the `./.nupkg` directory

### Install *BuildSphere* as a local tool

- From your project directory
  - create a manifest file `dotnet new tool-manifest`
  - run `dotnet tool install --add-source ../BuildSphere/.nupkg buildsphere`


- From your

