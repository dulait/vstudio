using VStudio.Core.Services;

namespace VStudio.Core.UnitTests;

public class SolutionFinderTests : IDisposable
{
    private readonly string _tempDir;
    private readonly SolutionFinder _sut;

    public SolutionFinderTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);
        _sut = new SolutionFinder();
    }

    public void Dispose()
    {
        if (!Directory.Exists(_tempDir))
        {
            return;
        }

        Directory.Delete(_tempDir, recursive: true);
    }

    [Fact]
    public void FindSolutions_NonExistentDirectory_ThrowsDirectoryNotFoundException()
    {
        var fakePath = Path.Combine(_tempDir, "does_not_exist");

        Assert.Throws<DirectoryNotFoundException>(() => _sut.FindSolutions(fakePath));
    }

    [Fact]
    public void FindSolutions_EmptyDirectory_ReturnsEmptyList()
    {
        var result = _sut.FindSolutions(_tempDir);

        Assert.Empty(result);
    }

    [Fact]
    public void FindSolutions_DirectoryWithSlnFile_ReturnsSolutionFile()
    {
        File.WriteAllText(Path.Combine(_tempDir, "MyApp.sln"), "");

        var result = _sut.FindSolutions(_tempDir);

        var solution = Assert.Single(result);
        Assert.Equal("MyApp.sln", solution.FileName);
        Assert.False(solution.IsSlnx);
    }

    [Fact]
    public void FindSolutions_DirectoryWithSlnxFile_ReturnsSolutionFile()
    {
        File.WriteAllText(Path.Combine(_tempDir, "MyApp.slnx"), "");

        var result = _sut.FindSolutions(_tempDir);

        var solution = Assert.Single(result);
        Assert.Equal("MyApp.slnx", solution.FileName);
        Assert.True(solution.IsSlnx);
    }

    [Fact]
    public void FindSolutions_DirectoryWithBothTypes_ReturnsBoth()
    {
        File.WriteAllText(Path.Combine(_tempDir, "App.sln"), "");
        File.WriteAllText(Path.Combine(_tempDir, "App.slnx"), "");

        var result = _sut.FindSolutions(_tempDir);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void FindSolutions_ResultsOrderedByFileName()
    {
        File.WriteAllText(Path.Combine(_tempDir, "Zebra.sln"), "");
        File.WriteAllText(Path.Combine(_tempDir, "Alpha.sln"), "");
        File.WriteAllText(Path.Combine(_tempDir, "Middle.slnx"), "");

        var result = _sut.FindSolutions(_tempDir);

        Assert.Equal(3, result.Count);
        Assert.Equal("Alpha.sln", result[0].FileName);
        Assert.Equal("Middle.slnx", result[1].FileName);
        Assert.Equal("Zebra.sln", result[2].FileName);
    }

    [Fact]
    public void FindSolutions_IgnoresNonSolutionFiles()
    {
        File.WriteAllText(Path.Combine(_tempDir, "MyApp.sln"), "");
        File.WriteAllText(Path.Combine(_tempDir, "readme.md"), "");
        File.WriteAllText(Path.Combine(_tempDir, "project.csproj"), "");

        var result = _sut.FindSolutions(_tempDir);

        var solution = Assert.Single(result);
        Assert.Equal("MyApp.sln", solution.FileName);
    }

    [Fact]
    public void FindSolutions_DoesNotRecurseIntoSubdirectories()
    {
        var subDir = Path.Combine(_tempDir, "subproject");
        Directory.CreateDirectory(subDir);
        File.WriteAllText(Path.Combine(subDir, "Nested.sln"), "");

        var result = _sut.FindSolutions(_tempDir);

        Assert.Empty(result);
    }

    [Fact]
    public void FindSolutions_ReturnsFullPaths()
    {
        File.WriteAllText(Path.Combine(_tempDir, "MyApp.sln"), "");

        var result = _sut.FindSolutions(_tempDir);

        var solution = Assert.Single(result);
        Assert.Equal(Path.Combine(_tempDir, "MyApp.sln"), solution.FullPath);
    }
}
