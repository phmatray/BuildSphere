using System.IO;
using System.Linq;

public interface IInitBuildSphere : INukeBuildSphere
{
    AbsolutePath NukeDirectory
        => RootDirectory / ".nuke";
    
    AbsolutePath NukeParametersFile
        => NukeDirectory / "parameters.json";

    [UsedImplicitly]
    Target Init => _ => _
        .Executes(() =>
        {
            Information("Initializing build sphere...");

            // Create the .nuke directory if it doesn't exist
            if (!NukeDirectory.Exists())
            {
                Information("Creating .nuke directory...");
                NukeDirectory.CreateDirectory();
            }
            
            // Create the .nuke/parameters.json file if it doesn't exist
            if (!NukeParametersFile.Exists())
            {
                // Locate the solution file
                var solutionFile = RootDirectory
                    .GlobFiles("*.sln")
                    .FirstOrDefault();

                if (solutionFile == null)
                {
                    throw new FileNotFoundException("Solution file not found.");
                }
            
                Information("Solution file located at: {0}", solutionFile);
            
                var parametersFileContent =
                    $$"""
                      {
                          "$schema": "build.schema.json",
                          "Solution": "{{solutionFile}}"
                      }
                      """;
            
                File.WriteAllText(NukeParametersFile, parametersFileContent);
                
                Information("Created .nuke/parameters.json file.");
            }
            
            Information("Build sphere initialized successfully!");
        });
}