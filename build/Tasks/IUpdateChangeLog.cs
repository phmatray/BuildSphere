using System.IO;
using System.Linq;
using System.Reflection;
using Nuke.Common.Tooling;
using static Nuke.Common.Tools.Git.GitTasks;

public interface IUpdateChangeLog : INukeBuild
{
    /// <summary>
    /// Path to the CHANGELOG.md file.
    /// </summary>
    AbsolutePath ChangelogFile
        => RootDirectory / "CHANGELOG.md";

    [UsedImplicitly]
    Target GenerateChangeLog => _ => _
        .Executes(() =>
        {
            // Step 1: Validate git-cliff Installation
            Information("Validating git-cliff installation...");
            var gitCliffPath =
                ToolPathResolver.GetPathExecutable("git-cliff")
                ?? throw new Exception("git-cliff is not installed or not found in PATH. " +
                                       " Please install git-cliff and ensure it's available in your system's PATH.");

            // Step 2: Check if git-cliff configuration file exists
            Information("Retrieving embedded git-cliff.toml configuration...");
            
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly.GetManifestResourceNames();

            Information("Embedded Resources:");
            foreach (var resourceName in resourceNames)
            {
                Information(resourceName);
            }
            
            // Dynamically find the embedded resource ending with 'git-cliff.toml'
            var targetResource = resourceNames
                .FirstOrDefault(name => name.EndsWith("git-cliff.toml", StringComparison.OrdinalIgnoreCase));

            if (targetResource == null)
            {
                throw new FileNotFoundException("Embedded resource 'git-cliff.toml' not found.");
            }
            
            var tempCliffConfig = TemporaryDirectory / "git-cliff.toml";
            using (var resourceStream = assembly.GetManifestResourceStream(targetResource))
            {
                if (resourceStream == null)
                {
                    throw new FileNotFoundException($"Failed to load embedded resource '{targetResource}'.");
                }

                using (var fileStream = File.Create(tempCliffConfig))
                {
                    resourceStream.CopyTo(fileStream);
                }
            }

            Information($"git-cliff.toml extracted to temporary location: {tempCliffConfig}");

            // Step 3: Generate CHANGELOG.md using git-cliff
            Information("Generating CHANGELOG.md using git-cliff...");
            var gitCliffArguments = $"--config \"{tempCliffConfig}\" --output \"{ChangelogFile}\"";
            var gitCliffProcess = ProcessTasks.StartProcess(gitCliffPath, gitCliffArguments);
            gitCliffProcess.AssertZeroExitCode();
            
            // Step 4: Stage the CHANGELOG.md file
            Git($"add \"{ChangelogFile}\"");
            
            // Step 5: Check if CHANGELOG.md was modified
            Information("Checking if CHANGELOG.md has been updated...");
            
            // Get the relative path of CHANGELOG.md from the repository root
            var changelogRelativePath = ChangelogFile
                .GetRelativePathTo(RootDirectory)
                .ToUnixRelativePath();
            
            // Execute 'git diff --name-only HEAD' to get changed files in the last commit
            var changedFiles = Git("diff --name-only HEAD")
                .Select(output => output.Text.Trim())
                .ToList();

            var changelogChanged = changedFiles.Contains(changelogRelativePath);
            
            if (changelogChanged)
            {
                Information("CHANGELOG.md has been updated. Preparing to commit the changes...");


                // Step 6: Create a new commit with a meaningful message
                const string commitMessage = "chore: update CHANGELOG.md";
                Git($"commit -m \"{commitMessage}\"");

                Information($"Committed changes to {ChangelogFile} with message: '{commitMessage}'");
            }
            else
            {
                Information("No changes detected in CHANGELOG.md. No commit necessary.");
            }
        });
}