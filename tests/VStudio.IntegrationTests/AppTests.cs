using Spectre.Console;
using VStudio.Cli;
using VStudio.Core.Services;

namespace VStudio.IntegrationTests;

public class AppTests : IDisposable
{
    private readonly string _tempDir;
    private readonly FakeLauncher _fakeLauncher;
    private readonly IAnsiConsole _console;

    public AppTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);
        _fakeLauncher = new FakeLauncher();
        _console = AnsiConsole.Create(new AnsiConsoleSettings
        {
            Out = new AnsiConsoleOutput(TextWriter.Null),
        });
    }

    public void Dispose()
    {
        if (!Directory.Exists(_tempDir))
        {
            return;
        }

        Directory.Delete(_tempDir, recursive: true);
    }

    private App CreateApp() => new(new SolutionFinder(), _fakeLauncher, _console);

    [Fact]
    public void DirectSlnPath_LaunchesSolution()
    {
        var slnPath = Path.Combine(_tempDir, "MyApp.sln");
        File.WriteAllText(slnPath, "");

        var exitCode = CreateApp().Run([slnPath]);

        Assert.Equal(0, exitCode);
        Assert.NotNull(_fakeLauncher.LaunchedSolution);
        Assert.Equal(slnPath, _fakeLauncher.LaunchedSolution.FullPath);
    }

    [Fact]
    public void DirectSlnxPath_LaunchesSolution()
    {
        var slnxPath = Path.Combine(_tempDir, "MyApp.slnx");
        File.WriteAllText(slnxPath, "");

        var exitCode = CreateApp().Run([slnxPath]);

        Assert.Equal(0, exitCode);
        Assert.NotNull(_fakeLauncher.LaunchedSolution);
        Assert.Equal(slnxPath, _fakeLauncher.LaunchedSolution.FullPath);
    }

    [Fact]
    public void DirectoryWithSingleSolution_LaunchesSolution()
    {
        var slnPath = Path.Combine(_tempDir, "MyApp.sln");
        File.WriteAllText(slnPath, "");

        var exitCode = CreateApp().Run([_tempDir]);

        Assert.Equal(0, exitCode);
        Assert.NotNull(_fakeLauncher.LaunchedSolution);
        Assert.Equal("MyApp.sln", _fakeLauncher.LaunchedSolution.FileName);
    }

    [Fact]
    public void NonExistentPath_ReturnsExitCode1()
    {
        var fakePath = Path.Combine(_tempDir, "nonexistent");

        var exitCode = CreateApp().Run([fakePath]);

        Assert.Equal(1, exitCode);
        Assert.Null(_fakeLauncher.LaunchedSolution);
    }

    [Fact]
    public void NonSolutionFile_ReturnsExitCode1()
    {
        var txtFile = Path.Combine(_tempDir, "readme.txt");
        File.WriteAllText(txtFile, "");

        var exitCode = CreateApp().Run([txtFile]);

        Assert.Equal(1, exitCode);
        Assert.Null(_fakeLauncher.LaunchedSolution);
    }

    [Fact]
    public void EmptyDirectory_ReturnsExitCode1()
    {
        var exitCode = CreateApp().Run([_tempDir]);

        Assert.Equal(1, exitCode);
        Assert.Null(_fakeLauncher.LaunchedSolution);
    }
}
