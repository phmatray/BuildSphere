using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.MinVer;

public interface INukeBuildSphere : INukeBuild
{
    [Solution(SuppressBuildProjectCheck = true)]
    Solution Solution
        => TryGetValue(() => Solution);
    
    [MinVer]
    MinVer MinVer
        => TryGetValue(() => MinVer);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    Configuration Configuration
        => IsLocalBuild ? Configuration.Debug : Configuration.Release;
}