public interface ICompileProjects : INukeBuildSphere
{
    Target Clean => _ => _
        .Executes(() =>
        {
            Information("Cleaning project directories...");
            
            RootDirectory
                .GlobDirectories("**/bin", "**/obj")
                .ForEach(directory =>
                {
                    Information($"Cleaning {directory}");
                    directory.DeleteDirectory();
                });
            
            Information("Project directories cleaned successfully!");
        });
    
    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s.SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            Information("Solution              : {0}", Solution);
            Information("Configuration         : {0}", Configuration);
            Information("MinVer Version        : {0}", MinVer.Version);
            Information("MinVer AssemblyVersion: {0}", MinVer.AssemblyVersion);
            Information("MinVer FileVersion    : {0}", MinVer.FileVersion);
            
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore()
                .SetDeterministic(true)
                // Inject versioning properties
                .SetVersion(MinVer.Version)
                .SetAssemblyVersion(MinVer.AssemblyVersion)
                .SetFileVersion(MinVer.FileVersion));
        });
    
    [UsedImplicitly]
    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .EnableNoRestore());
        });
}