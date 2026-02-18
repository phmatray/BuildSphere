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

            // Create the .nuke/parameters.json file if it doesn't exist
            if (!NukeParametersFile.Exists())
            {
                Information("Creating .nuke/parameters.json file...");
                
                // Locate the solution file
                var solutionFile = RootDirectory
                    .GlobFiles("*.sln")
                    .FirstOrDefault();

                if (solutionFile == null)
                {
                    throw new FileNotFoundException("Solution file not found.");
                }
            
                RelativePath solutionFileRelativePath = RootDirectory.GetRelativePathTo(solutionFile);
                
                Information("Solution file located at: {0}", solutionFileRelativePath);
            
                var parametersFileContent =
                    $$"""
                      {
                          "$schema": "build.schema.json",
                          "Solution": "{{solutionFileRelativePath}}"
                      }
                      """;
            
                File.WriteAllText(NukeParametersFile, parametersFileContent);
                
                Information("Created .nuke/parameters.json file.");
            }
            
            Information("Build sphere initialized successfully!");
        });
}