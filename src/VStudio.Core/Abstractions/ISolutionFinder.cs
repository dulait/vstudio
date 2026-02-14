using VStudio.Core.Models;

namespace VStudio.Core.Abstractions;

public interface ISolutionFinder
{
    IReadOnlyList<SolutionFile> FindSolutions(string directoryPath);
}
