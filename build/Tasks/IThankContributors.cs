using System.Linq;
using System.Text;
using Nuke.Common.Utilities;
using static Nuke.Common.Tools.Git.GitTasks;

public interface IThankContributors : INukeBuild
{
    AbsolutePath ContributorsFile
        => RootDirectory / "CONTRIBUTORS.md";
    AbsolutePath ContributorsCacheFile
        => TemporaryDirectory / "contributors.dat";

    [UsedImplicitly]
    Target UpdateContributors => _ => _
        .Executes(() =>
        {
            var previousContributors = ContributorsCacheFile.Existing()?.ReadAllLines() ?? [];
            
            var repositoryDirectory = RootDirectory / ".git";
            var contributors = Git("log --pretty=%an|%ae%n%cn|%ce", repositoryDirectory, logOutput: false)
                .Select(x => x.Text)
                .Distinct()
                .ToList()
                .Select(x => x.Split('|'))
                .ForEachLazy(x => Assert.Count(x, length: 2))
                .Select(x => new { Name = x[0], Email = x[1] })
                .ToList();

            var newContributors = contributors.Where(x => !previousContributors.Contains(x.Email));

            foreach (var newContributor in newContributors)
            {
                var content = (ContributorsFile.Existing()?.ReadAllLines() ?? [])
                    .Concat($"- {newContributor.Name}")
                    .OrderBy(x => x);
                ContributorsFile.WriteAllLines(content, Encoding.Default);
                Git($"add {ContributorsFile}");

                var message = $"chore: add {newContributor.Name} as contributor".DoubleQuote();
                var author = $"{newContributor.Name} <{newContributor.Email}>".DoubleQuote();
                Git($"commit -m {message} --author {author}");
            }

            ContributorsCacheFile.WriteAllLines(contributors.Select(x => x.Email).ToList());
        });
}