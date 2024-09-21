public interface IHandleNuGetPackage : INukeBuildSphere
{
    [Parameter("NuGet API Key")]
    string NuGetApiKey
        => TryGetValue(() => NuGetApiKey);

    [Parameter("NuGet Source URL")]
    string NuGetSource
        => TryGetValue(() => NuGetSource)
           ?? "https://api.nuget.org/v3/index.json";

    AbsolutePath ArtifactsDirectory
        => RootDirectory / "artifacts";
    
    Target Pack => _ => _
        .Executes(() =>
        {
            DotNetPack(s => s
                .SetProject(Solution)
                .SetConfiguration(Configuration)
                .SetOutputDirectory(ArtifactsDirectory)
                .EnableNoBuild()
                .EnableNoRestore()
                .SetIncludeSymbols(true)
                .SetIncludeSource(true)
                .SetSymbolPackageFormat(DotNetSymbolPackageFormat.snupkg)
                // Use MinVer version for the package
                .SetVersion(MinVer.Version)
                .SetAssemblyVersion(MinVer.AssemblyVersion)
                .SetFileVersion(MinVer.FileVersion));
        });
    
    [UsedImplicitly]
    Target Publish => _ => _
        .DependsOn(Pack)
        .Requires(() => NuGetApiKey)
        .Executes(() =>
        {
            Information("NuGet API Key : {0}", NuGetApiKey);
            Information("NuGet Source  : {0}", NuGetSource);
            
            ArtifactsDirectory
                .GlobFiles("*.nupkg")
                .ForEach(package =>
                {
                    DotNetNuGetPush(s => s
                        .SetTargetPath(package)
                        .SetSource(NuGetSource)
                        .SetApiKey(NuGetApiKey)
                        .EnableSkipDuplicate());
                });

            ArtifactsDirectory
                .GlobFiles("*.snupkg")
                .ForEach(symbolsPackage =>
                {
                    DotNetNuGetPush(s => s
                        .SetTargetPath(symbolsPackage)
                        .SetSource("https://api.nuget.org/v3/index.json")
                        .SetApiKey(NuGetApiKey)
                        .EnableSkipDuplicate());
                });
        });
}