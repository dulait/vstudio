namespace VStudio.Core.Models;

public class SolutionFile
{
    public required string FullPath { get; init; }

    public string FileName => Path.GetFileName(FullPath);

    public string Extension => Path.GetExtension(FullPath);

    public bool IsSlnx => Extension.Equals(".slnx", StringComparison.OrdinalIgnoreCase);

    public override string ToString() => FileName;
}
