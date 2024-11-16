global using System;
global using JetBrains.Annotations;
global using Nuke.Common;
global using Nuke.Common.IO;
global using Nuke.Common.Tools.DotNet;
global using Nuke.Common.Utilities.Collections;
global using Serilog;
global using static Nuke.Common.Tools.DotNet.DotNetTasks;
global using static Serilog.Log;

// [GitHubActions(
//     "ci",
//     GitHubActionsImage.UbuntuLatest,
//     InvokedTargets = [nameof(Test)],
//     On =
//     [
//         GitHubActionsTrigger.Push,
//         GitHubActionsTrigger.WorkflowDispatch
//     ],
//     ImportSecrets = [nameof(NuGetApiKey)])]
class Build : NukeBuild,
    ICompileProjects,
    ITestJavaScript,
    IUpdateChangeLog,
    IGenerateDocs,
    IHandleNuGetPackage,
    IThankContributors
{
    public static int Main()
        => Execute<Build>(x => ((ICompileProjects)x).Compile);
}
