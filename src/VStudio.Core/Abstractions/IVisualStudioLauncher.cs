using VStudio.Core.Models;

namespace VStudio.Core.Abstractions;

public interface IVisualStudioLauncher
{
    void Launch(SolutionFile solutionFile);
}
