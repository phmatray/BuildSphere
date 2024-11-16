using System.Linq;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Npm;
using static Nuke.Common.Tools.Npm.NpmTasks;

public interface ITestJavaScript : INukeBuildSphere
{
    Target TestJest => _ => _
        .Executes(() =>
        {
            Information("Testing JavaScript with Jest...");
            
            // Redirect NPM output to Serilog
            NpmLogger = (outputType, text) => Log.Information(text);
        
            // Define the path to the Jest tests directory
            var jestTestProjects = RootDirectory
                .GlobDirectories("**/*.JsTests")
                .Select(directory => directory / "package.json")
                .ToList();
            
            if (jestTestProjects.Count == 0)
            {
                Information("No Jest tests found!");
                return;
            }

            Information("Found Jest tests:");
            jestTestProjects.ForEach(TestJestDirectory);
        });

    void TestJestDirectory(AbsolutePath jestTestsDirectory)
    {
        Information("Testing Jest directory: {0}", jestTestsDirectory);
        
        // Install NPM dependencies
        Information("Installing NPM dependencies...");
        NpmInstall(new NpmInstallSettings()
            .SetProcessWorkingDirectory(jestTestsDirectory));
        
        // Run Jest tests
        Information("Running Jest tests...");
        NpmRun(new NpmRunSettings()
            .SetCommand("test")
            .SetProcessWorkingDirectory(jestTestsDirectory));
        
        Information("Jest directory tests completed successfully!");
    }
}