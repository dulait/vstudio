using VStudio.Core.Abstractions;
using VStudio.Core.Models;

namespace VStudio.Core.Services;

public class SolutionFinder : ISolutionFinder
{
    public IReadOnlyList<SolutionFile> FindSolutions(string directoryPath)
    {
        var fullPath = Path.GetFullPath(directoryPath);

        if (!Directory.Exists(fullPath))
        {
            throw new DirectoryNotFoundException($"Directory not found: {fullPath}");
        }

        var slnFiles = Directory.GetFiles(fullPath, "*.sln", SearchOption.TopDirectoryOnly);
        var slnxFiles = Directory.GetFiles(fullPath, "*.slnx", SearchOption.TopDirectoryOnly);

        return slnFiles
            .Concat(slnxFiles)
            .Order(StringComparer.OrdinalIgnoreCase)
            .Select(path => new SolutionFile { FullPath = path })
            .ToList()
            .AsReadOnly();
    }
}
