using VStudio.Core.Abstractions;
using VStudio.Core.Models;

namespace VStudio.IntegrationTests;

internal class FakeLauncher : IVisualStudioLauncher
{
    public SolutionFile? LaunchedSolution { get; private set; }

    public void Launch(SolutionFile solutionFile)
    {
        LaunchedSolution = solutionFile;
    }
}
