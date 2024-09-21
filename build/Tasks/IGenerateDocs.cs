using Nuke.Common.Tools.DocFX;
using static Nuke.Common.Tools.DocFX.DocFXTasks;

public interface IGenerateDocs : INukeBuild
{
    AbsolutePath DocfxConfig
        => RootDirectory / "docfx.json";
    
    AbsolutePath DocfxSite
        => RootDirectory / "_site";
    
    [UsedImplicitly]
    Target GenerateDocs => _ => _
        // .DependsOn(Pack)
        .Executes(() =>
        {
            Information("Generating documentation with DocFX...");

            // Build documentation
            DocFXBuild(s => s
                .SetConfigFile(DocfxConfig)
                .SetOutputFolder(DocfxSite));

            Information("Documentation generated successfully at '{0}'.", DocfxSite);
        });
}